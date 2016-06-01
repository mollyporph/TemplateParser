# TemplateParser

A simple parser for joining templates together using reference-tags. Works with any type of file and content,
as long as the reference start-tag and end-tag is unique. 
Supports recursion, pointing to a folder with multiple files, and using simple wildcards in the reference string e.g <#REFERENCE=multiplefiles/*.json#>

Requires dotnet cli for RC2 or higher

For instance if you have the following files:

index.html
```
<html>
<body>
  <#REFERENCE=body.html#>
</body>
```
body.html
```
<div>
  <#REFERENCE=texts/sometext.txt#>
</div>
<ul>
  <#REFERENCE=listobjects#>
</ul>
```

texts/sometext.txt
```
Hello World!
```

listobjects/1.html
```
<li> First</li>
```

listobjects/2.html
```
<li> Second </li>
```

Run with `dotnet run index.html`
You can also supply and optional outfile with `dotnet run index.html outputfile.html`

The resulting file will look like this:

```
<html>
<body>
  <div>
  Hello World!
</div>
<ul>
  <li> First</li>
  <li> Second </li>
</ul>
</body>
```
