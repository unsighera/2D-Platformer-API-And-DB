using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ApplyPlatformDefines
{
    public static string[] Paths = { "Assets\\Plugins\\Plugins\\SignalR" };
    public const string Extension = "t:Object";
    public const string NetStandard = "NET_STANDARD";
    public const string NotNetStandard = "!NET_STANDARD";

    [MenuItem(itemName: "Tools/Apply SignalR Constraints")]
    public static void ApplyDefines()

    {
        //������� Unity, ��� �� �������� �������������� ������� � ���� �� �� ��������, 
        //�� ����� ������������� �� � ������� ����� ����� ������� �� ���
        AssetDatabase.StartAssetEditing();
        //��� ����� ������ ���������� ������ ������� �� ������ �������
        //AssetDatabase.StopAssetEditing(); ����� ��������� ��������� �������
        try
        {
            Execute();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void Execute()
    {
        var assets = AssetDatabase.FindAssets(Extension, Paths);
        var constraints = new List<string>();
        foreach (var guid in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            //�������� �� ���� � ������ ��� �������� ��� ���������� ��� �����������
            var importer = AssetImporter.GetAtPath(path);
            //��� ���������� ������ ��������� dll, ��� � unity ���� ��� PluginImporter
            //������� ����� ������ ��
            if (importer is not PluginImporter pluginImporter) continue;

            var restriction = path.Contains("netstandard") ? NetStandard : NotNetStandard;

            //��������� ����� ������ ����������� ��� dll
            constraints.Clear();
            constraints.AddRange(pluginImporter.DefineConstraints);
            constraints.RemoveAll(
                x => x.Equals(NetStandard, StringComparison.OrdinalIgnoreCase) ||
                     x.Equals(NotNetStandard, StringComparison.OrdinalIgnoreCase));

            constraints.Add(restriction);
            //������ ����� ����������� �� ����, � ����� ����� ��������� dll
            pluginImporter.DefineConstraints = constraints.ToArray();

            Debug.Log($"ADD constraints {restriction} to {path}");

            //��������� ���������
            pluginImporter.SaveAndReimport();
        }
    }
}