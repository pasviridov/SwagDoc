# Swagger specification documentation generator

This tool generates plain documentation file by Swagger specification.

Usage:
```
    SwagDoc [-o <output_file_path>] <json_spec_file_path>
```

If `-o` option isn't specified specification path used with changed extension.

Target file format is detecting by its extension. Default extension (without `-o` option) is ".docx".
Only Microsoft Word 2016 is now supported (it must be installed on the computer).

Followed options are supported in the configuration file:
```
<configuration>
  <swagDoc>
    <msword template="<The path to the Word document used as a template. If it isn't specified, new document will be created.>"
            showApplication="<true|false - Should the Word application be visible during generating process.>"
            groupResources="<true|false - Should REST resources be groupped by their first tags.>">
      <captions commonGroup="<The name of group of untagged REST resources, if any (in case of groupping applied).>"
                pathParameters="<The caption text for path parameters.>"
                queryParameters="<The caption text for query parameters.>"
                requestBody="<The caption text for request body.>"
                responseBody="<The caption text for response body.>" />
      <styles resourceGroupTitle="<The local name of style for resource group titles>"
              resourceTitle="<The local name of style for resource titles>"
              resourceRequest="<The local name of style for request signature (e.g POST /api/my-resource)>"
              responseTitle="<The local name of style for response titles>"
              description="<The local name of style for descriptions>"
              caption="<The local name of style for parameter, request & response captions>"
              content="<The local name of style for request & response bodies>" />
    </msword>
  </swagDoc>
</configuration>
```

Followed placeholders can be used in the template document:
- {{swagdoc.title}} for specification title.
- {{swagdoc.version}} for specification version.
- {{swagdoc.description}} for specification description.
- {{swagdoc.resources}} for entire documentation. If it isn't specified, documentation will be placed at the end of the document.

(c) Pavel Sviridov (pasviridov@gmail.com)
