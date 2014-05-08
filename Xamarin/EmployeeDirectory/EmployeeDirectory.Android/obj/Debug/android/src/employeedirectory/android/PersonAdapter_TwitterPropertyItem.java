package employeedirectory.android;


public class PersonAdapter_TwitterPropertyItem
	extends employeedirectory.android.PersonAdapter_UrlPropertyItem
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("EmployeeDirectory.Android.PersonAdapter/TwitterPropertyItem, EmployeeDirectory.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PersonAdapter_TwitterPropertyItem.class, __md_methods);
	}


	public PersonAdapter_TwitterPropertyItem () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PersonAdapter_TwitterPropertyItem.class)
			mono.android.TypeManager.Activate ("EmployeeDirectory.Android.PersonAdapter/TwitterPropertyItem, EmployeeDirectory.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public PersonAdapter_TwitterPropertyItem (int p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == PersonAdapter_TwitterPropertyItem.class)
			mono.android.TypeManager.Activate ("EmployeeDirectory.Android.PersonAdapter/TwitterPropertyItem, EmployeeDirectory.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
