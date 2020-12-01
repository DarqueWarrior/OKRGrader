# OKR Grader Function

This repository contains the code for an Azure Function that responds to a 'Work item updated' web hook from Azure DevOps (AzD). The web hook is configured to fire when the Grade field of a custom work item type named Key Result is changed. When the grade is changed on a key result the hook fires and the function is called. The function uses the JSON sent with the request to locate the objective that needs to be graded. From there it finds all the linked key results and sets the Average field on the objective (another custom work item type).

## Contents

The repository has three projects.

1. OKRGrader: This project is a collection of classes used to parse the payload into usable objects. I did not want to use dynamic types and wanted nice POCO to use instead. The project also contains the Grader static class. The Grader contains all the logic to grade an objective from the grades of the linked key results.

2. OKRGrader.Tests: The unit test project to make sure the POCO parse correctly.

3. OKRGraderFunctionApp: This is the function that responds to the web hook from AzD.

## Local Development

This code was developed with [Visual Studio 2017 Version 15.9.4](https://visualstudio.microsoft.com/?WT.mc_id=devops-0000-dbrown) with latest Azure Function and Web Job Tools Version 15.9.02046.0. However, the project is all .NET Core and can be built on any platform running [.NET Core 2.1](http://dot.net).

To work on your development machine add a `local.settings.json` file with the following contents to the OKRGraderFunctionApp folder.

```json
{
   "IsEncrypted": false,
   "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet",
      "PAT": "Enter your AzD Personal Access Token Here"
   }
}
```

You will also need the [Azure Storage Emulator](https://docs.microsoft.com/azure/storage/common/storage-use-emulator?WT.mc_id=devops-0000-dbrown).