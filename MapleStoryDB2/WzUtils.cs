using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WzComparerR2.WzLib;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Wz.Utils
{
    public class Arc
    {
        public static Wz_Structure MobWz, Mob001Wz, Mob2Wz, MapWz, Map2Wz, Map001Wz, Map002Wz, SkillWz, Skill001Wz, Skill002Wz,
         CharacterWz, ItemWz, NpcWz, MorphWz, StringWz, EtcWz, ReactorWz,UIWz,SoundWz,Sound001Wz,Sound2Wz,EffectWz,BaseWz;
    }

    public static class WzUtils
    {

        public static string IDString(this string Str)
        {
            return int.Parse(Str).ToString();

        }

        public static string GetPathD(this Wz_Node Node)
        {
            Stack<string> Path = new Stack<string>();
            Wz_Node ThisNode = Node;
            do
            {
                Path.Push(ThisNode.Text);
                ThisNode = ThisNode.ParentNode;
            } while (ThisNode != null);
            return string.Join(".", Path.ToArray());

        }

        public static string GetPath(this Wz_Node Node)
        {
            Stack<string> Path = new Stack<string>();
            Wz_Node ThisNode = Node;
            do
            {
                Path.Push(ThisNode.Text);
                ThisNode = ThisNode.ParentNode;
            } while (ThisNode != null);
            return string.Join("/", Path.ToArray());

        }


        public static string ImgName(this Wz_Node Node)
        {

            return Node.GetNodeWzImage().Name;

        }

        public static string ImgID(this Wz_Node Node)
        {

            return Node.GetNodeWzImage().Name.Replace(".img", "");

        }
        public static Wz_Node FindNodeByPathA(this Wz_Node Node, string FullPath, bool ExtractImage)
        {

            string[] Patten = FullPath.Split('/');
            // if (Node != null)       
            return Node.FindNodeByPath(ExtractImage, Patten);
            //return null;
        }



        public static Wz_Node GetNode(this Wz_Node Node, string Path)
        {
           // Wz_Node Result = null;
            if (Node.FindNodeByPathA(Path, true) != null)
            {
                if (Node.FindNodeByPathA(Path, true).Value is Wz_Uol)
                    return Node.FindNodeByPathA(Path, true).ResolveUol();
                else
                    return Node.FindNodeByPathA(Path, true);

            }
            else
            {

                string[] Split = Path.Split('/');
                int Count = 0;
                string Str = "";
                string Path1 = "";
                string Path2 = "";
                bool HasUol = false;
                for (int i = 0; i < Split.Length; i++)
                {

                    if (i == 0)
                        Str = Str + Split[i];
                    else
                        Str += '/' + Split[i];
                    if ((Node.FindNodeByPathA(Str, true) != null) && (Node.FindNodeByPathA(Str, true).Value is Wz_Uol))
                    {
                        HasUol = true;
                        Count = i;
                        Path1 = Str;
                        break;
                    }

                }

                if (HasUol)
                {
                    Str = "";
                    for (int i = Count + 1; i < Split.Length; i++)
                    {
                        if (i == Count + 1)
                            Str = Str + Split[i];
                        else
                            Str = Str + '/' + Split[i];
                        Path2 = Str;
                    }
                    return Node.FindNodeByPathA(Path1, true).ResolveUol().FindNodeByPathA(Path2, true);
                }
            }
            return null;

        }

        public static bool HasNode(this Wz_Node Node, string Path)
        {
            return Node.GetNode(Path) != null;

        }

        public static Bitmap ExtractPng2(this Wz_Node Node)
        {
            if (Node.HasNode("_outlink"))
            {
                var LinkData = Node.GetNode("_outlink").Value.ToString();
                string[] Split = LinkData.Split('/');
                string DestPath = "";
                switch (Split[0])
                {
                    case "Mob":
                        DestPath = Regex.Replace(LinkData, "Mob/", "");
                        if (Arc.MobWz != null && Arc.MobWz.HasNode(Split[1]))
                            return (Arc.MobWz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        else if (Arc.Mob001Wz != null && Arc.Mob001Wz.HasNode(Split[1]))
                            return (Arc.Mob001Wz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        else
                            return (Arc.Mob2Wz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        break;

                    case "Map":
                        if (Split[1] == "Map")
                            DestPath = LinkData.Remove(0, 4);
                        else
                            DestPath = Regex.Replace(LinkData, "Map/", "");

                        if (Arc.MapWz != null && Arc.MapWz.HasNode(Split[1] + "/" + Split[2]))
                            return (Arc.MapWz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        else if (Arc.Map001Wz != null && Arc.Map001Wz.HasNode(Split[1] + "/" + Split[2]))
                            return (Arc.Map001Wz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        else if (Arc.Map002Wz != null && Arc.Map002Wz.HasNode(Split[1] + '/' + Split[2]))
                            return (Arc.Map002Wz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        else
                            return (Arc.Map2Wz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        break;
                    case "Skill":
                        DestPath = Regex.Replace(LinkData, "Skill/", "");
                        if (Arc.SkillWz != null && Arc.SkillWz.HasNode(Split[1]))
                            return (Arc.SkillWz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        else if (Arc.Skill001Wz != null && Arc.Skill001Wz.HasNode(Split[1]))
                            return (Arc.Skill001Wz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        else
                            return (Arc.Skill002Wz.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        break;
                    default:
                        DestPath = Regex.Replace(LinkData, Node.GetNodeWzFile().Type.ToString() + "/", "");
                        return (Node.GetNodeWzFile().WzStructure.GetNode(DestPath).Value as Wz_Png).ExtractPng();
                        break;

                };

            }
            else if (Node.HasNode("_inlink"))
            {
                var LinkData = Node.GetNode("_inlink").Value.ToString();
                return (Node.GetNodeWzImage().Node.GetNode(LinkData).Value as Wz_Png).ExtractPng();

            }
            else if (Node.HasNode("source"))
            {
                var LinkData = Node.GetNode("source").Value.ToString();
                var DestPath = Regex.Replace(LinkData, Node.GetNodeWzFile().Type.ToString() + "/", "");
                return (Node.GetNodeWzFile().WzStructure.GetNode(DestPath).Value as Wz_Png).ExtractPng();

            }
            else
            {
                if (Node.Value is Wz_Uol)
                    return (Node.ResolveUol().Value as Wz_Png).ExtractPng();
                else
                    return (Node.Value as Wz_Png).ExtractPng();

            }


            return null;
        }

        public static T GetValue2<T>(this Wz_Node Node, string Path, T DefaultValue)
        {
            if (Node.FindNodeByPathA(Path, true) != null)
                return Node.FindNodeByPathA(Path, true).GetValueEx(DefaultValue);
            else
                return DefaultValue;
        }

        public static int ValueToInt(this Wz_Node Node)
        {

            return Node.GetValueEx<int>(0);

        }

        public static string ValueToStr(this Wz_Node Node)
        {

            return Node.GetValueEx<string>("");

        }
       
        public static string RightStr(string s, int count)
        {
            if (count > s.Length)
                count = s.Length;
            return s.Substring(s.Length - count, count);
        }


       

    }


    public static class WzUtils2
    {
        public static Wz_Node GetNode(this Wz_Structure Wz, string Path)
        {
            return Wz.WzNode.GetNode(Path);

        }

        public static bool HasNode(this Wz_Structure Wz, string Path)
        {
            return Wz.WzNode.GetNode(Path) != null;

        }
        public static WzComparerR2.WzLib.Wz_Node.WzNodeCollection Nodes(this Wz_Structure Self)
         {
            return Self.WzNode.Nodes;

        }

    }




}
