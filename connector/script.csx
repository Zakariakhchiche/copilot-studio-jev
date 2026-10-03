public class Script : ScriptBase
{
    // The simple operations (AskYesNo, Classify, Rate) are posted to /v1/systemone/{type}.
    // This script turns them into one System One request with a single typed question,
    // sends it to /v1/systemone, and flattens the answer. Evaluate and ListModels pass through.
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        switch (this.Context.OperationId)
        {
            case "AskYesNo":
            case "Classify":
            case "Rate":
                return await this.SendSimpleQuestion().ConfigureAwait(false);
            default:
                return await this.Context.SendAsync(this.Context.Request, this.CancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<HttpResponseMessage> SendSimpleQuestion()
    {
        var input = JObject.Parse(await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false));
        var operation = this.Context.OperationId;

        var question = new JObject { ["instructions"] = input["question"] };
        if (operation == "AskYesNo")
        {
            question["type"] = "noul";
            var criteria = new JObject();
            if (!string.IsNullOrEmpty((string)input["yes_means"])) criteria["true"] = input["yes_means"];
            if (!string.IsNullOrEmpty((string)input["no_means"])) criteria["false"] = input["no_means"];
            if (criteria.HasValues) question["criteria"] = criteria;
        }
        else if (operation == "Classify")
        {
            question["type"] = "choice";
            var criteria = new JObject();
            foreach (var option in (JArray)input["options"] ?? new JArray())
            {
                var name = (string)option["name"];
                if (string.IsNullOrEmpty(name)) continue;
                var description = (string)option["description"];
                criteria[name] = string.IsNullOrEmpty(description) ? JValue.CreateNull() : new JValue(description);
            }
            question["criteria"] = criteria;
        }
        else
        {
            question["type"] = "score";
            question["criteria"] = input["levels"] ?? new JArray();
        }

        var model = (string)input["model"];
        var request = new JObject
        {
            ["state"] = input["state"],
            ["model"] = string.IsNullOrEmpty(model) ? "jev-latest" : model,
            ["questions"] = new JObject { ["q"] = question },
        };

        var uri = new UriBuilder(this.Context.Request.RequestUri) { Path = "/v1/systemone" };
        this.Context.Request.RequestUri = uri.Uri;
        this.Context.Request.Content = CreateJsonContent(request.ToString());

        var response = await this.Context.SendAsync(this.Context.Request, this.CancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        var body = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        var answer = body["answers"]?["q"];
        var output = new JObject
        {
            ["model"] = body["model"],
            ["input_tokens"] = body["usage"]?["input_tokens"],
        };

        if (operation == "AskYesNo")
        {
            output["probability"] = answer?["noul"];
        }
        else if (operation == "Classify")
        {
            output["choice"] = answer?["choice"];
            output["confidence"] = answer?["confidence"];
            var probabilities = new JArray();
            foreach (var p in (answer?["probabilities"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>())
            {
                probabilities.Add(new JObject { ["option"] = p.Name, ["probability"] = p.Value });
            }
            output["probabilities"] = probabilities;
        }
        else
        {
            output["score"] = answer?["score"];
            output["confidence"] = answer?["confidence"];
            // Most likely level: the level with the highest probability, mapped through the legend.
            var best = (answer?["probabilities"] as JObject)?.Properties()
                .OrderByDescending(p => (double)p.Value)
                .FirstOrDefault();
            if (best != null)
            {
                output["level"] = answer?["legend"]?[best.Name];
            }
        }

        response.Content = CreateJsonContent(output.ToString());
        return response;
    }
}
