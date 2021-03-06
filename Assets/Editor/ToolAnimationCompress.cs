﻿using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
public class ToolAnimationCompress : MonoBehaviour {
	private static int number = 0;
	static string ExePath =  "F:/FBXCompress/Release/FBXCompress.exe";
	static DateTime time;
	[MenuItem("AnimationTool/CompressClip")]
	static void Execute()
	{
		number = 0;
		time = DateTime.Now;
		foreach (UnityEngine.Object o in Selection.GetFiltered(typeof(AnimationClip), SelectionMode.DeepAssets))
		{
			number++;
			CompressAnimationClip((Instantiate(o) as AnimationClip), EditorUtility.GetAssetPath(o));
//			ToolYamlReader.ParseAnimationFile(EditorUtility.GetAssetPath(o));
		}
		AssetDatabase.SaveAssets();
		Log("一共压缩了"+number+"个动画文件!");
		Log("耗时:"+((DateTime.Now-time).TotalMilliseconds/1000)+"秒.");
	}

	[MenuItem("AnimationTool/CompressFBX")]
	static void ExecuteFBX()
	{
		string RootPath =  Application.dataPath;
		RootPath = RootPath.Substring(0,RootPath.LastIndexOf("/"))+"/";
		Directory.CreateDirectory("C:/FBXCompress/");
		StreamWriter writer =File.CreateText("C:/FBXCompress/FBX.txt");
		foreach (UnityEngine.Object o in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets))
		{
			string fbxPath = RootPath+EditorUtility.GetAssetPath(o);
			if(fbxPath.Contains(".FBX"))
			{
				writer.WriteLine(fbxPath);
			}
		}
		writer.Flush();
		writer.Close();
		Process.Start(ExePath);
    }

	static void CompressAnimationClip(AnimationClip clip, string clipName)
	{
		AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip);
		Keyframe key;
		Keyframe[] keyFrames;
		foreach(AnimationClipCurveData curveDate in curves)
		{
			keyFrames = curveDate.curve.keys;
			for(int i=0;i<keyFrames.Length;i++)
			{
				key = keyFrames[i];
				key.value = float.Parse(key.value.ToString("f3"));
				key.inTangent = float.Parse(key.inTangent.ToString("f3"));
				key.outTangent = float.Parse(key.outTangent.ToString("f3"));
				keyFrames[i] = key;
			}
			curveDate.curve.keys = keyFrames;
			clip.SetCurve(curveDate.path,curveDate.type,curveDate.propertyName,curveDate.curve);
		}
		AssetDatabase.CreateAsset(clip, clipName);
	}

	static void Log(string content)
	{
		UnityEngine.Debug.Log(content);
	}
}