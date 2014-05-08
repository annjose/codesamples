package employeedirectory.android;


public class Application
	extends mono.android.app.Application
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:()V:GetOnCreateHandler\n" +
			"";
	}


	public Application () throws java.lang.Throwable
	{
		super ();
	}

	public void onCreate ()
	{
		mono.android.Runtime.register ("EmployeeDirectory.Android.Application, EmployeeDirectory.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Application.class, __md_methods);
		n_onCreate ();
	}

	private native void n_onCreate ();

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
