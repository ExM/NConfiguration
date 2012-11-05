using System;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace Configuration
{
	public static class KeyManager
	{
		public const string KeyContainerName = "MyTestKeys";

		public const string XmlKey = @"<RSAKeyValue><Modulus>vOw2mws8u0fnPIS9fUg0ocyzAv/FdoJvohiHO6y6n5fEJhhXek809JTnocnSBipMb7rE18PUKe+UbBJZcxRAZePsGGTE54B/c1moEZDyJudOiHwiKOoA0/VshbArsuCAeC5GyX6IiMqiZqt5UbIVzvzxjYJ4lx3DkE/+rjEx1mL5jMp2FTXLQo2rlg9v5akqFGYbVMjLArxTQRS+hcCZQL0/0m4d0DaUYyg4T8qKWvcbzqQKvbNmHOTCBrS2J40Qj+7qe8WUxKGARKDKz0eDYk4FM2KyTKPyYtGPtuzzmNuUPJq8HuxLYHctcfKMx/yLuxCLemirC2eyuT0wqS2KOQ==</Modulus><Exponent>AQAB</Exponent><P>/iFuPI8saXertI++w9UvpsxGatVmIZtlALb6UaH1Uc1Jg+tky5aAqtoembcrGbf2+hvBTQmlTKcyWICHw3aurF3/2ofoE5XNMw+a6R8YwL5TZCq5XZ/mImnS1yHjB9nyxtXxQ53aKVS8e81/uD5ZvNhARn7nHlZQiCQ0W8h9rZc=</P><Q>vk/8UMK/lw9VIT9oZ6D5JO/tZvWED1JzP27kqc5YvYs2D6dBpIaXpyeDVgq/iScVKMgcDrSDQIk5oyHrZCTfB0dHPvo3rpE9qdoiaWBOZ+wQdxQcu3WJ3EmRoWlm7tVWJVwNBfpRC+F41EifkSCs+2GbT7VHWqCB62+ZbbsXIK8=</Q><DP>AeNvtUV/F/2KqhnTWhMwenXUJCEX1DIaawnxDEA+2W/EFxXdtuGUbTCXv56r1FIYpL0mD5N9xfcMGbpCyAIOxDsilpYh9FtNng6EoSzY+z2u0vS6UebJSAIvTefzjvBgrHeUhmMa5um5SNY7F7xm4E9fEucIgqIzkP9r4C66OFs=</DP><DQ>ICbjOcGtHJg6iaNswBUyAkuGkB5qcSw8zDqv5wA1fdBHEd3v+RvEAzlptt+Z/FHeAUXNd66Hfh+w3R6d5g1UoKYqIzmirptbD5cKuULL/EKlhXigYgs5fz3unJZyDWd9ZlJm4NupVZiCEVszmhErKE0VMCIwK99yE5SHF0LEZYE=</DQ><InverseQ>DIYzq/5CI9YpJj97Ns5WQhFJgu1cCMrIQjAr93yJNmB9kpLw+f9EiKvlRwe00NEHUTHfJgE5/RcQ1eo9pMcq3WS10tRe6c0sZVzUAgK7ISr/sxwQVZoVVWZPNcgNpOsmS/k5a2w3T4FGnShPL4S6N2o4xt0vgYiJw3nKfmNxekQ=</InverseQ><D>iPKmJr//lzbpLZp/jdiQppUvUsY7ysuExrkHFsAATH4EZyUQDI5sMbvbKRGWmHeDDx8RX+MJ4hhUu/6VLvGIORP+ajRp8/LW8LQWPB2ZG5BjAlcRoBz42q02rLZnBvTsy7GoCJobuSVCkjBM8maonnIHW/AvIEQoJm2GjZkFqoS0eiqNHTtZpZjSwqxOJ5GjUkWpvzJI+UnXe8r+DocH5v0F5voFNSmDLxTbdVTQhahwJig+ZOmuk8R88+H9xggnv8ZEmyOUSD4RTmpEJYrZ2CX7odVln3medRUcTbqJoXt5Kq/76SkAu0MhhlXfsK86UZglEHJRKVCDBMz7tWpBHQ==</D></RSAKeyValue>";

		public static void Create()
		{
			var cp = new CspParameters();
			cp.KeyContainerName = KeyContainerName;
			cp.Flags = CspProviderFlags.UseMachineKeyStore;
			
			var rsa = new RSACryptoServiceProvider(cp);
			rsa.FromXmlString(XmlKey);
			rsa.PersistKeyInCsp = true;
			rsa.Clear();
		}


		public static void Delete()
		{
			var cp = new CspParameters();
			cp.KeyContainerName = KeyContainerName;
			cp.Flags = CspProviderFlags.UseMachineKeyStore;
			var rsa = new RSACryptoServiceProvider(cp);
			rsa.PersistKeyInCsp = false; // Delete the key entry in the container.
			rsa.Clear();
		}
	}
}



