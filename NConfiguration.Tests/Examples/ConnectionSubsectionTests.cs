using System;
using System.Linq;
using NConfiguration.Tests;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using NConfiguration.Xml;
using NConfiguration.Joining;
using NConfiguration.Examples;
using NConfiguration.Xml.Protected;
using NConfiguration.Serialization;
using NConfiguration.Json;
using NConfiguration.Combination;

namespace NConfiguration.Examples
{
	[TestFixture]
	public class ConnectionSubsectionTests
	{
		[Test]
		public void DefaultConnection()
		{
			var settings = @"<Config>
<Connections>
	<Default server='localhost' database='workDb' user='admin' password='pass' />
	<Temp />
</Connections>
</Config>".ToXmlSettings().ToAppSettings();


			var connections = settings.Subsection("connections");

			var defaultConn = connections.Get<AutoCombinableConnectionConfig>("default");
			var tempConn = connections.Combine(defaultConn, connections.Get<AutoCombinableConnectionConfig>("temp"));

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", defaultConn.ConnectionString);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", tempConn.ConnectionString);
		}

		[Test]
		public void OwerrideInOneSection()
		{
			var settings = @"<Config>
<Connections>
	<Default server='localhost' database='workDb' user='admin' password='pass' />
	<Temp database='tempDb' />
</Connections>
</Config>".ToXmlSettings().ToAppSettings();


			var connections = settings.Subsection("connections");

			var defaultConn = connections.Get<AutoCombinableConnectionConfig>("default");
			var tempConn = connections.Combine(defaultConn, connections.Get<AutoCombinableConnectionConfig>("temp"));

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", defaultConn.ConnectionString);
			Assert.AreEqual("Server=localhost;Database=tempDb;User ID=admin;Password=pass;", tempConn.ConnectionString);
		}

		[Test]
		public void OwerrideInManySection()
		{
			var settings = @"<Config>
<Connections>
	<Default database='workDb' />
	<Temp database='tempDb' />
</Connections>
<Connections>
	<Default user='admin' password='pass' />
	<Temp />
</Connections>
<Connections>
	<Default server='default_server' />
	<Temp server='temp_server' />
</Connections>
</Config>".ToXmlSettings().ToAppSettings();


			var connections = settings.Subsection("connections");

			var defaultConn = connections.Get<AutoCombinableConnectionConfig>("default");
			var tempConn = connections.Combine(defaultConn, connections.Get<AutoCombinableConnectionConfig>("temp"));

			Assert.AreEqual("Server=default_server;Database=workDb;User ID=admin;Password=pass;", defaultConn.ConnectionString);
			Assert.AreEqual("Server=temp_server;Database=tempDb;User ID=admin;Password=pass;", tempConn.ConnectionString);
		}
	}
}

