# TypeSafe

TypeSafe System One models, such as Jev, return calibrated, structured decisions instead of generated text: the probability that a yes/no question is true, one option chosen from a list you define, or a score on a scale you define. In Copilot Studio, Power Automate and Power Apps, use them to classify and route requests, gate retrieved passages before an agent answers, check content against a rule, or score quality, and decide in your flow or topic what to do with the probability and confidence.

## Publisher: Zakaria Khchiche

## Prerequisites

A TypeSafe account and API key. Sign up at [console.typesafe.ai](https://console.typesafe.ai/). Usage is billed by TypeSafe per input token; see [Models](https://docs.typesafe.ai/models) for current pricing and rate limits.

## Supported Operations

### Ask a yes/no question
Evaluates a yes/no question against the content and returns the probability, from 0 to 1, that the answer is yes. Optionally describe what a yes and a no mean.

### Choose one option
Picks one option from a list you define (2 to 255 options, each with an optional description) and returns the chosen option, its confidence and the probability of every option.

### Rate on a scale
Rates the content on an ordered scale you define (2 to 10 levels, lowest first) and returns a probability-weighted score, the most likely level and the confidence.

### Evaluate questions (advanced)
Sends a full System One request with several typed questions (`noul`, `choice`, `score`) about the same content and returns one answer per question. See the [API reference](https://docs.typesafe.ai/api).

### List models
Lists the model names and aliases your account can use.

## Obtaining Credentials

1. Sign in to [console.typesafe.ai](https://console.typesafe.ai/).
2. Create an API key.
3. When you create a connection, paste the key in the **API key** field. The connector sends it as `Authorization: Bearer <key>`.

## Getting Started

In Copilot Studio, add the connector as a tool and describe when the agent should call it, for example: "Use Ask a yes/no question to check whether a retrieved passage answers the user's question before answering." In Power Automate, compare the returned probability or confidence with a threshold in a condition, and send low-confidence items to a person.

## Known Issues and Limitations

- English is Jev's primary language. Other languages are supported with lower accuracy: measure on your own content before relying on a threshold.
- Each request accepts up to 64k tokens in total and 32k tokens for the content plus the longest question.
- The `jev-latest` alias moves when a new model ships. Set the Model field to a versioned ID, such as `jev-1.13.0`, if you tune thresholds on its output.
- Rate limits are set by TypeSafe and can return `429` or `529`; retry with a delay.

## Deployment Instructions

Deploy with the [Power Platform Connectors CLI](https://learn.microsoft.com/en-us/connectors/custom-connectors/paconn-cli):

```bash
paconn create --api-def apiDefinition.swagger.json --api-prop apiProperties.json --script script.csx
```
