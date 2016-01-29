using System;
using System.Collections.Generic;
using System.Collections;

public interface IResource
{
	// what type of object will GetAsset return
	Type ResourceType { get; }

	//how much memory does this asset take up when loaded
	int ByteCount { get; }

	// shorte name of file
	string Name { get; }
	// full path to file
	string Path{ get; }

	// load the asset into memory if it isnt already, then return it
	object GetAsset ();

	// signifies that a client is done using an asset
	//void ReleseAsset ();

	// is anyone using this asset in memory
	//bool Unused();

	//load the asset into memory
	void Load();
	// non blocking load
	IEnumerator WaitLoad();

	// unload asset from memory. will only work if Unused is true
	void Unload ();
	// non blocking unload
	IEnumerator WaitUnload();
}




