# Wrike Power Automate Custom Connector

This project provides the necessary components to create a custom connector for the Wrike API in Microsoft Power Automate. It includes a Swagger (OpenAPI) definition file and custom C# code to handle specific requirements of the Wrike API.

## Components

- **`WrikeAPI.swagger.yaml`**: The OpenAPI definition file for the Wrike API. This file is used to configure the custom connector in Power Automate, defining the various actions and triggers available.
- **`customcode.cs`**: A C# script used as custom code within the Power Automate connector. This script modifies the outgoing API requests to ensure they are compatible with the Wrike API, particularly for handling parameters that need to be sent as JSON arrays.
- **`LICENSE`**: The license for this project.

## Features

The custom connector supports a variety of Wrike API operations, including:

- **Create Folder**: Create a new folder or project.
- **Get Folder Tree**: Retrieve the folder structure for a given folder.
- **Get Tasks**: Get tasks from a folder, project, or space.
- **Modify Tasks**: Modify one or more tasks.
- **Blueprint Launch**: Launch a new project from a blueprint.

...and many more! A complete and up-to-date list of all supported operations will be available soon on [leothelegion.net](https://leothelegion.net).

### Custom Code Enhancements

The custom code in `customcode.cs` performs two main functions:

1.  **Removes Null Parameters**: It cleans up the request URL by removing any query parameters that have null or empty values.
2.  **Converts Parameters to JSON Arrays**: The Wrike API expects certain parameters (like `fields`, `status`, and `addParents`) to be in the format of a JSON array (e.g., `["value1", "value2"]`). Power Automate, however, typically passes multi-value parameters as comma-separated strings (e.g., `value1,value2`). The script intercepts these parameters and converts them into the correct JSON array format before sending the request to the Wrike API.

## How to Use

1.  **Create a Custom Connector in Power Automate**:
    - Navigate to the Power Automate portal.
    - Go to **Data > Custom connectors**.
    - Click on **New custom connector** and select **Import an OpenAPI file**.
2.  **Import the Swagger File**:
    - Give your connector a name (e.g., "Wrike Custom").
    - Upload the `WrikeAPI.swagger.yaml` file.
    - Continue through the setup wizard.
3.  **Configure Authentication**:
    - In the **2. Security** section of the custom connector setup, you will need to configure the authentication method.
    - Select **OAuth 2.0** as the authentication type.
    - For the **Identity Provider**, choose **Generic Oauth 2**.
    - You will need to provide your **Client ID** and **Client secret**. You can get these by creating an application in your Wrike account. For more details, see the [Wrike documentation on OAuth 2.0 Authentication](https://developers.wrike.com/documentation/oauth-2-0-authentication).
    - **Authorization URL**: `https://login.wrike.com/oauth2/authorize/v4`
    - **Token URL**: `https://login.wrike.com/oauth2/token`
    - **Refresh URL**: `https://login.wrike.com/oauth2/token`
    - **Scope**: You can leave this blank to request all scopes, or specify the scopes you need. `ws-read-write` is a good default for general access.
4.  **Add the Custom Code**:
    - In the **4. Code** section of the custom connector setup, enable the **Code enabled** option.
    - Copy the entire content of the `customcode.cs` file and paste it into the code editor.
    - The class in the script is `Script`, which inherits from `ScriptBase`, so you don't need to change the default class name.
5.  **Create and Test the Connector**:
    - Click **Create connector**.
    - Once the connector is created, you can test the various actions in the **5. Test** section to ensure it's working correctly.

You can now use this custom connector in your Power Automate flows to automate your Wrike workflows.

## Video Tutorial

For a visual guide on how to set up and use this custom connector, you can watch this video. Please note that the video may be slightly out of date, but it should still provide a good overview of the process.:

[Wrike Custom Connector for Power Automate](https://youtu.be/inva0HZCDK4)
