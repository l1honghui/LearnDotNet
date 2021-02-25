using System;
using System.IO;
using System.IO.Compression;

namespace Common
{
    class Compress
    {
		#region GZip

		private static byte[] GZipCompress(byte[] data)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
				{
					gZipStream.Write(data, 0, data.Length);
				}
				return stream.ToArray();
			}
		}

		private static byte[] GZipDecompress(byte[] data)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
				{
					byte[] bytes = new byte[40960];
					int n;
					while ((n = gZipStream.Read(bytes, 0, bytes.Length)) != 0)
					{
						stream.Write(bytes, 0, n);
					}
				}
				return stream.ToArray();
			}
		}
        #endregion

    }
}
