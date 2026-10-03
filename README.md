# Copilot Studio × Jev (TypeSafe)

**Agents Copilot Studio qui répondent sur un très grand corpus documentaire uniquement quand un document le justifie, et qui le disent quand aucun document ne le justifie.**

Ce dépôt rassemble deux briques pour utiliser [Jev](https://docs.typesafe.ai/), le modèle « System One » de TypeSafe, dans Microsoft Copilot Studio et la Power Platform :

| Brique | Ce qu'elle fait | Statut |
|---|---|---|
| [`mcp-server/`](./mcp-server/) | Serveur MCP (TypeScript) : recherche Azure AI Search, puis Jev pose 4 questions oui/non sur chaque passage (pertinent, contient la réponse, contredit la question, tente une injection) ; le code décide : preuve, contradiction ou rejet. L'agent répond avec ses sources, signale la contradiction ou s'abstient. | Proposé à Microsoft : [microsoft/CopilotStudioSamples#539](https://github.com/microsoft/CopilotStudioSamples/pull/539) (brouillon) |
| [`connector/`](./connector/) | Connecteur Power Platform « TypeSafe » : *Ask a yes/no question*, *Choose one option*, *Rate on a scale*, *Evaluate questions*, *List models*. Utilisable comme outil dans Copilot Studio et dans Power Automate. | Préparé pour le programme Independent Publisher de [microsoft/PowerPlatformConnectors](https://github.com/microsoft/PowerPlatformConnectors) |

## Pourquoi

Sur des milliers de documents, la recherche remonte souvent la bonne procédure pour le mauvais modèle d'équipement, une révision périmée, ou un texte qui tente de donner des instructions à l'agent. Le re-classement change l'ordre, pas la décision. Ici, Jev renvoie des probabilités calibrées par passage et le code applique des seuils explicites : changer la politique, c'est modifier un nombre relu en revue de code, pas réécrire un prompt.

Cas d'usage de démonstration : assistant maintenance et sécurité pour techniciens terrain d'une régie d'eau fictive (Contoso Water).

## Démarrer

- Serveur MCP : voir [`mcp-server/README.md`](./mcp-server/README.md) (tests hors ligne : `npm test`, aucune clé nécessaire).
- Connecteur : voir [`connector/readme.md`](./connector/readme.md) (`paconn create ...`).

## Limites à connaître

- Jev est d'abord entraîné en anglais ; les autres langues, dont le français, sont prises en charge avec une précision moindre. Mesurer sur son propre corpus avant de fixer les seuils.
- Le filtre anti-injection est un filtre, pas une frontière de sécurité.
- Épingler la version du modèle (`jev-1.13.0`) si l'on règle des seuils.

---

## English

Two building blocks to use TypeSafe Jev in Microsoft Copilot Studio and the Power Platform: an MCP server that gates Azure AI Search results with four calibrated yes/no questions per passage so the agent answers with citations, flags a false premise, or abstains (proposed upstream in [microsoft/CopilotStudioSamples#539](https://github.com/microsoft/CopilotStudioSamples/pull/539)), and a Power Platform custom connector for the TypeSafe System One API.

---

## Auteur

**Zakaria Khchiche**, Tech Lead Data & IA freelance (Paris), formateur IA pour Spar-x (organisme certifié Qualiopi, financement OPCO possible).

- LinkedIn : https://www.linkedin.com/in/zakariakhchiche/
- Site : https://zakariakhchiche.github.io/
- Formation Copilot Studio : https://zakariakhchiche.github.io/formation-copilot-studio/
- Demande de devis formation : https://mte.typeform.com/spar-xv3?typeform-source=www.spar-x.fr

Licence : MIT. Projet indépendant, non affilié à Microsoft ni à TypeSafe.
