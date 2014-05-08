package employeedirectory.android;


public class PeopleGroupsAdapter_GroupHeaderItem
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_toString:()Ljava/lang/String;:GetToStringHandler\n" +
			"";
		mono.android.Runtime.register ("EmployeeDirectory.Android.PeopleGroupsAdapter/GroupHeaderItem, EmployeeDirectory.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PeopleGroupsAdapter_GroupHeaderItem.class, __md_methods);
	}


	public PeopleGroupsAdapter_GroupHeaderItem () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PeopleGroupsAdapter_GroupHeaderItem.class)
			mono.android.TypeManager.Activate ("EmployeeDirectory.Android.PeopleGroupsAdapter/GroupHeaderItem, EmployeeDirectory.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public java.lang.String toString ()
	{
		return n_toString ();
	}

	private native java.lang.String n_toString ();

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
