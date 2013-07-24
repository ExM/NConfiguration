NConfiguration
===

Library for simple configuration management

Source code:
 git://github.com/ExM/NConfiguration.git

 
Examples:
---

In your `web.config` or `app.config` make sure you have a custom section:

```xml
<configuration>
	<configSections>
		<section name="ExtConfigure" type="NConfiguration.PlainXmlSection, NConfiguration"/>
	</configSections>
	<ExtConfigure>
		<Include>
			<XmlFile Path="etc\externalConfig.xml" Search="All" Include="All" Required="true"/>
			<XmlFile Path="etc\machineSpecific.xml" Search="All" Include="All" Required="true"/>
		</Include>
	</ExtConfigure>

</configuration>
```

Find and download all of the files in the parent directory.
Tracking changed included files.

```c# 
public IAppSettings LoadSettings()
{
	var strConv = new StringConverter();
	var deserializer = new GenericDeserializer();

	var xmlFileLoader = new XmlFileSettingsLoader(deserializer, strConv);
	var loader = new SettingsLoader(xmlFileLoader);

	loader.LoadSettings(new XmlSystemSettings("ExtConfigure", strConv, deserializer));

	var result = loader.Settings;
	result.Changed += SettingsChanged;
	return result;
}

private void SettingsChanged(object sender, EventArgs e)
{
	//TODO: reload settings or application
}
```

Deserialization section with the name of all downloaded files.

```c# 
var configs = settings.LoadCollection<ConfigClass>("MyConfig")
```
