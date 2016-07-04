using System;
using System.IO;
using System.Threading;

namespace NConfiguration.Monitoring
{
	public sealed class AsyncComparer
	{
		private const int ChunkSize = 0x4000;

		private long _totalBytes = 0;
		private readonly byte[] _buffer = new byte[ChunkSize];
		private readonly byte[] _expected;
		private Stream _src;
		private Action<bool> _completed;

		private AsyncComparer(Stream source, byte[] expected, Action<bool> completed)
		{
			_expected = expected;
			_src = source;
			_completed = completed;

			_src.BeginRead(_buffer, 0, ChunkSize, onRead, null);
		}

		public static void Compare(string fileName, byte[] expected, Action<bool> completed)
		{
			try
			{
				var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				if (fs.Length == expected.Length)
				{
					if (expected.Length != 0)
					{
						new AsyncComparer(fs, expected, completed);
						return;
					}

					ThreadPool.QueueUserWorkItem(trueResultWork, completed);
					return;
				}
				
				fs.Dispose();
			}
			catch (Exception)
			{
			}

			ThreadPool.QueueUserWorkItem(falseResultWork, completed);
		}

		private static void trueResultWork(object arg)
		{
			((Action<bool>)arg)(true);
		}

		private static void falseResultWork(object arg)
		{
			((Action<bool>)arg)(false);
		}

		private void onRead(IAsyncResult readResult)
		{
			bool result = true;
			try
			{
				int readed = _src.EndRead(readResult);

				for (int i = 0; i < readed; i++)
				{
					if (_expected[_totalBytes] != _buffer[i])
					{
						result = false;
						break;
					}
					_totalBytes++;
				}

				if(result && readed > 0)
				{
					_src.BeginRead(_buffer, 0, ChunkSize, onRead, null);
					return;
				}
			}
			catch(Exception)
			{
				result = false;
			}

			_src.Dispose();
			_completed(result);
		}
	}
}
