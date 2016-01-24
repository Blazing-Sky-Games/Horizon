using Slash.Unity.DataBind.Core.Data;

public class DataBindTestContext : Context
{
	private readonly Property<string> textProperty = new Property<string>();

	public DataBindTestContext()
	{
		Text = "databind for unity";
	}

	public string Text
	{
		get
		{
			return this.textProperty.Value;
		}
		set
		{
			this.textProperty.Value = value;
		}
	}

}
