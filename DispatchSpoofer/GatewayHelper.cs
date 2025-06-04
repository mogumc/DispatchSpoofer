using DispatchSpoofer;
using KazusaProtoBuf.ProtoModule;

public class GatewayHelper
{
	private readonly byte[] _original;
	private string? MdkResVersion;
	private string? IfixVersion;
	private bool MdkResVersion_set = false;
	private bool IfixVersion_set = false;
	private List<ProtoField> removeFields = new List<ProtoField>();

	public GatewayHelper(byte[] original)
	{
		_original = original;
		// Console.WriteLine(Convert.ToHexString(original));
	}

	public byte[] Process()
	{
		HotfixJson jsonData = MainApp._hotfixJson;
		ProtoMessage msg = ProtoMessage.FromByteArray(_original);
		foreach (ProtoField field in msg._fields)
		{
			// Console.WriteLine(field.Tag);
			switch (field.Value.GetType().FullName)
			{
				case "System.String":
					string valStr = (string)field.Value;

					// God, forvie me for all those ifs...
					if (valStr.Contains("/asb/"))
					{
						field.Value = jsonData.assetBundleUrl;
					}
					else if (valStr.Contains("/design_data/"))
					{
						field.Value = jsonData.exResourceUrl;
					}
					else if (valStr.Contains("/lua/"))
					{
						field.Value = jsonData.luaUrl;
						MdkResVersion = valStr.Split("output_").Last().Split("_").First();
					}
					else if (valStr.Contains("/ifix/"))
					{
						field.Value = jsonData.ifixUrl;
						IfixVersion = valStr.Split("output_").Last().Split("_").First();
					}

					break;

				default:
					// Console.WriteLine($"WTF {field.Value.GetType()}");
					break;
			}

			// iteration for ifix and mdkres
			foreach (ProtoField field2 in msg._fields)
			{
				// Console.WriteLine(field.Tag);
				switch (field.Value.GetType().FullName)
				{
					case "System.String":

						if (field.Value.ToString() == "")
							removeFields.Add(field);

						if (!UInt64.TryParse(field.Value.ToString(), out ulong ulongVal))
							break;

						if (MdkResVersion != null && ulongVal.ToString() == MdkResVersion && !MdkResVersion_set)
						{
							field.Value = jsonData.customMdkResVersion.ToString();
						}
						else if (IfixVersion != null && ulongVal.ToString() == IfixVersion && !IfixVersion_set)
						{
							field.Value = jsonData.customIfixVersion.ToString();
						}

						break;
				}
			}

			foreach (ProtoField protoField in removeFields)
			{
				msg._fields.Remove(protoField);
			}
		}

		return msg.ToByteArray();
	}
}