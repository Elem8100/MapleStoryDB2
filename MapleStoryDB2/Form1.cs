using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WzComparerR2.WzLib;
using WzComparerR2.PluginBase;
using System.Text.RegularExpressions;
using WzComparerR2;
using WzComparerR2.Common;
using WzComparerR2.CharaSimControl;
using WzComparerR2.CharaSim;
using System.Runtime.InteropServices;
using WzComparerR2.Controls;
using Wz.Utils;
using DataGrid;
using System.Threading;
using WzComparerR2.MapRender;
using Game = Microsoft.Xna.Framework.Game;

namespace WinFormsApp1
{


    public partial class Form1 : Form
    {
        List<string> ColList, ColList1, RowList;
        Dictionary<int, List<string>> RowList1;
        int Row1 = -1;
        int tabIndex = 0;
        DataViewer Grid;
        DataViewer SearchGrid;

        void Dump2(Wz_Node Entry)
        {
            if (Entry != null)
            {
                if (Entry.Value is Wz_Vector)
                {
                    var P = Entry.GetValue<Wz_Vector>();
                    ColList.Add(Entry.GetPathD() + "=" + P.X.ToString() + "," + P.Y.ToString() + ",  ");

                }
                else if (Entry.GetValue("Null") != "Null")
                    ColList.Add(Entry.GetPathD() + "=" + Entry.GetValueEx<string>("-") + ",  ");
                foreach (var E in Entry.Nodes)
                    if (!(E.Value is Wz_Png))
                        Dump2(E);
            }
        }

        void Delete(string s, int index, int count)
        {
            if ((index < 1) | (index > s.Length) | (count <= 0))
                return;
            if (index + count - 1 > s.Length)
                count = s.Length - index + 1;
            s = s.Remove(index - 1, count);
        }
        void DumpData2(Wz_Node Entry)
        {

            Dump2(Entry);
            string FinalStr = "";
            var S = Entry.GetPathD() + ".";
            for (int i = 0; i < ColList.Count; i++)
            {
                ColList[i] = ColList[i].Replace(S, "");
                FinalStr = FinalStr + ColList[i];
            }
            Delete(FinalStr, FinalStr.Length - 2, 1);
            RowList.Add(FinalStr);
            ColList.Clear();
        }

        void DumpData1()
        {
            ColList1 = new List<string>();
            Row1++;
            RowList1.Add(Row1, ColList1);

        }

        void PutGridData1(int Col)
        {

            string[] FinalStr = new string[RowList1.Count + 1];
            foreach (var i in RowList1.Keys)
            {
                for (int j = 0; j < RowList1[i].Count; j++)
                    FinalStr[i] = FinalStr[i] + RowList1[i][j];
                // Delete(FinalStr[i], inttostr(Length(FinalStr[i])) , 1);
                Grid[Col, i].Value = FinalStr[i];
                //RowList1[i].Free;
            }

            RowList1.Clear();
            //    SetLength(FinalStr, 0);
        }


        void LoadItem()
        {
            var ItemDir = tabControl1.TabPages[tabIndex].Name;
            Wz_Node Child;
            switch (ItemDir)
            {
                case "Etc":
                    Child = Arc.StringWz.GetNode("Etc.img/Etc");
                    break;
                case "Install":
                    Child = Arc.StringWz.GetNode("Ins.img");
                    break;
                default:
                    Child = Arc.StringWz.GetNode(ItemDir + ".img");
                    break;

            }
            string ID;
            Bitmap Icon = null;
            foreach (var img in Arc.ItemWz.GetNode(ItemDir).Nodes)
            {
                if (ItemDir == "Pet")
                {
                    ID = img.ImgID();
                    if (Arc.ItemWz.GetNode("Pet/" + img.Text + "/info/iconD") != null)
                        Icon = Arc.ItemWz.GetNode("Pet/" + img.Text + "/info/iconD").ExtractPng2();
                    var Name = Child.GetValue2(ID + "/name", "  ");
                    var Desc = Child.GetValue2(ID + "/desc", "  ");
                    DumpData2(Arc.ItemWz.GetNode("Pet/" + img.Text + "/info"));
                    Grid.Rows.Add(ID, Icon, Name, Desc, "");
                }
                else
                {
                    foreach (var Iter in Arc.ItemWz.GetNode(ItemDir + "/" + img.Text).Nodes)
                    {
                        DumpData2(Iter);
                        ID = Iter.Text.IDString();
                        if (Iter.GetNode("info/icon") != null)
                            Icon = Iter.GetNode("info/icon").ExtractPng2();
                        Grid.Rows.Add(Iter.Text, Icon, Child.GetValue2(ID + "/name", "  "), Child.GetValue2(ID + "/desc", "  "), " ");
                    }
                }

            }
            for (int i = 0; i < RowList.Count; i++)
                Grid[4, i].Value = RowList[i];
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);

        }

        string GetTypes(string ID)
        {
            switch (int.Parse(ID) / 10000)
            {
                case 101: return "種類: 臉飾,"; break;
                case 102: return "種類: 眼飾,"; break;
                case 103: return "種類: 耳環,"; break;
                case 112: return "種類: 墜飾,"; break;
                case 113: return "種類: 腰帶,"; break;
                case 114: return "種類: 勳章,"; break;
                case 115: return "種類: 肩飾,"; break;
                case 116: return "種類: 口袋道具,"; break;
                case 118: return "種類: 胸章,"; break;
                case 119: return "種類: 徽章,"; break;
                case 121: return "種類: 閃亮克魯,"; break;
                case 122: return "種類: 靈魂射手"; break;
                case 123: return "種類: 魔劍,"; break;
                case 124: return "種類: 能量劍,"; break;
                case 125: return "種類: 幻獸棍棒,"; break;
                case 130: return "種類: 單手劍,"; break;
                case 131: return "種類: 單手斧,"; break;
                case 132: return "種類: 單手棍,"; break;
                case 133: return "種類: 短劍,"; break;
                case 134: return "種類: 雙刀,"; break;
                case 136: return "種類: 手杖,"; break;
                case 137: return "種類: 短杖,"; break;
                case 138: return "種類: 長杖,"; break;
                case 140: return "種類: 雙手劍,"; break;
                case 141: return "種類: 雙手斧,"; break;
                case 142: return "種類: 雙手棍,"; break;
                case 143: return "種類: 槍,"; break;
                case 144: return "種類: 矛,"; break;
                case 145: return "種類: 弓,"; break;
                case 146: return "種類: 弩,"; break;
                case 147: return "種類: 拳套,"; break;
                case 148: return "種類: 指虎,"; break;
                case 149: return "種類: 火槍,"; break;
                case 150: return "種類: 採藥,"; break;
                case 151: return "種類: 採礦,"; break;
                case 152: return "種類: 雙弩槍,"; break;
                case 153: return "種類: 加農砲,"; break;
                case 154: return "種類: 太刀,"; break;
                case 155: return "種類: 扇子,"; break;
                case 156: return "種類: 琉,"; break;
                case 157: return "種類: 璃,"; break;
                case 158: return "種類: 机甲手枪,"; break;
                case 159: return "種類: 古代之弓,"; break;
                case 170: return "種類: 點裝,"; break;
                case 194: return "種類:龍魔頭盔"; break;
                case 195: return "種類:龍魔項鍊"; break;
                case 196: return "種類:龍魔翅膀"; break;
                case 197: return "種類:龍魔尾巴"; break;
                case 135:
                    switch (int.Parse(ID.Trim()) / 10)
                    {
                        case 135200: return "種類:魔法箭,"; break;
                        case 135210: return "種類:卡牌,"; break;
                        case 135220: return "種類:墜飾,"; break;
                        case 135221: return "種類:念珠,"; break;
                        case 135222: return "種類:鐵鍊,"; break;
                        case 135223: return "種類:魔导书(火毒),"; break;
                        case 135224: return "種類:魔导书(冰雷),"; break;
                        case 135225: return "種類:魔导书(牧师),"; break;
                        case 135226: return "種類:箭失 ,"; break;
                        case 135227: return "種類:指虎,"; break;
                        case 135228: return "種類:短劍用劍套,"; break;
                        case 135229: return "種類:护身符,"; break;
                        case 135230: return "種類:寶盒,"; break;
                        case 135240: return "種類:寶石,"; break;
                        case 135250: return "種類:龍之精水,"; break;
                        case 135260: return "種類:靈魂之環,"; break;
                        case 135270: return "種類:連發槍,"; break;
                        case 135280: return "種類:太刀,"; break;
                        case 135281: return "種類:哨子,"; break;
                        case 135282: return "種類:拳爪,"; break;
                        case 135286: return "種類:拳環,"; break;
                        case 135290: return "種類:手腕护带,"; break;
                        case 135291: return "種類:望远镜 ,"; break;
                        case 135292: return "種類:火藥桶 ,"; break;
                        case 135293: return "種類:砝码,"; break;
                        case 135294: return "種類:文件,"; break;
                        case 135295: return "種類:魔法珠子,"; break;
                        case 135296: return "種類:箭矢,"; break;
                        case 135297: return "種類:寶石,"; break;
                        case 135298: return "種類:火藥桶,"; break;
                        case 135300: return "種類:控制器,"; break;
                        case 135310: return "種類:狐狸寶珠,"; break;
                        case 135320: return "種類:棋子,"; break;
                        case 135330: return "種類:武器傳送裝置,"; break;
                        case 135340: return "種類:裝填,"; break;
                        case 135350: return "種類:魔力翅膀,"; break;
                        case 135360: return "種類:精气珠,"; break;
                        case 135370: return "種類:遗物,"; break;
                        case 135400: return "種類:手鐲,"; break;
                        default: return ""; break;
                    }
                    break;
                default: return ""; break;
            }


            return null;
        }

        string ToData(Wz_Node E)
        {
            if (E.Value is Wz_Png)
                return null;
            switch (E.Text)
            {
                case "tradeAvailable":
                    if (E.ValueToInt() == 1) return "神奇剪刀";
                    if (E.ValueToInt() == 2) return "白金剪刀";
                    break;

                case "reqJob":
                    switch (E.ValueToInt())
                    {
                        case 0: return "全職業"; break;
                        case 1: return "劍士"; break;
                        case 2: return "法師"; break;
                        case 3: return "劍士&法師"; break;
                        case 4: return "弓箭手"; break;
                        case 8: return "盜賊"; break;
                        case 9: return "劍士&盜賊"; break;
                        case 13: return "劍士&弓手&盜賊"; break;
                        case 16: return "海盜"; break;
                        case 24: return "傑諾"; break;
                        default: return ""; break;

                    }
                    break;

                case "attackSpeed":
                    switch (E.ValueToInt())
                    {
                        case 3: return "更快(3)"; break;
                        case 4: return "快(4)"; break;
                        case 5: return "快(5)"; break;
                        case 6: return "普通(6)"; break;
                        case 7: return "慢(7)"; break;
                        case 8: return "慢(8)"; break;
                        case 9: return "比較慢(9)"; break;

                    }
                    break;

                case "bdR":
                case "incBDR":
                case "imdR":
                case "incIMDR":
                case "damR":
                case "nbdR":
                    return E.ValueToStr() + "%";
                    break;
                case "cash":
                    if (E.ValueToInt() == 0) return "";
                    if (E.ValueToInt() == 1) return "點裝";
                    break;
                case "addition":
                    if (E.HasNode("mobcategory"))
                        return "怪物剋星";
                    else if (E.HasNode("boss"))
                        return "頭目剋星";
                    else if (E.HasNode("skill"))
                        return "攻擊特效";
                    else if (E.HasNode("critical"))
                        return "會心一擊";
                    else if (E.HasNode("mobdie"))
                        return "獵殺特效";
                    else if (E.HasNode("statinc"))
                        return "追加能力";
                    break;
                case "variableStat":
                    if (E.HasNode("incPAD"))
                        return "攻撃力増加率：" + E.GetNode("incPAD").Value.ToString();
                    break;
                case "incRMAI":
                case "incRMAL":
                case "incRMAF":
                case "incRMAS":
                    return (E.ValueToInt() - 100).ToString() + "%";
                    break;
                default: return E.ValueToStr(); break;
            }

            return null;
        }

        string StrJoin(string Separator, params string[] StringArray)
        {
            var Result = "";
            for (int i = 0; i < StringArray.Length; i++)
                Result = Result + StringArray[i] + Separator;
            Delete(Result, Result.Length, 1);
            return Result;

        }
        string GetJobID(string ID)
        {
            return (int.Parse(ID) / 10000).ToString();
        }
        string LeftStr(string s, int count)
        {
            if (count > s.Length)
                count = s.Length;
            return s.Substring(0, count);
        }

        void LoadCharacter()
        {
            var ToName = new Dictionary<string, string>();
            var ToNameBool = new Dictionary<string, string>();
            ToName.Add("price", "賣價: ");
            ToName.Add("reqLevel", "REQ LEV: ");
            ToName.Add("reqJob", "適用職業: ");
            // ToName.Add("reqSpecJob", "");
            ToName.Add("reqSTR", "REQ STR: ");
            ToName.Add("reqDEX", "REQ DEX: ");
            ToName.Add("reqINT", "REQ INT: ");
            ToName.Add("reqLUK", "REQ LUK: ");
            ToName.Add("reqPOP", "名聲需求: ");
            ToName.Add("attackSpeed", "速度: ");
            ToName.Add("incSTR", "力量+");
            ToName.Add("incDEX", "敏捷+");
            ToName.Add("incINT", "智力+");
            ToName.Add("incLUK", "幸運+");
            ToName.Add("incMHP", "MaxHP+");
            ToName.Add("incMMP", "MaxMP+");
            ToName.Add("incPAD", "攻擊力+");
            ToName.Add("incMAD", "魔法攻擊力+");
            ToName.Add("incPDD", "防禦力+");
            ToName.Add("incMDD", "魔法防禦力+");
            ToName.Add("incACC", "命中值+");
            ToName.Add("incEVA", "迴避值+");
            ToName.Add("incSpeed", "移動速度+");
            ToName.Add("incJump", "跳躍力+");
            ToName.Add("incCraft", "手藝+");
            ToName.Add("craftEXP", "手藝經驗+");

            ToName.Add("incPVPDamage", "大亂鬥時增加攻擊力+");
            ToName.Add("bdR", "BOSS攻擊時傷害+");
            ToName.Add("imdR", "怪物防禦力無視+");
            ToName.Add("willEXP", "意志經驗+");
            ToName.Add("charmEXP", "魅力經驗+");
            ToName.Add("charismaEXP", "領導經驗+");
            ToName.Add("knockback", "直接打擊時的機率強弓:");
            ToName.Add("reduceReq", "套用等級減少:");

            ToName.Add("incRMAI", "冰系魔法+");
            ToName.Add("incRMAL", "雷系魔法+");
            ToName.Add("incRMAF", "火系魔法+");
            ToName.Add("incRMAS", "毒系魔法+");
            ToName.Add("durability", "耐久度: ");
            ToName.Add("tuc", "卷軸次數: ");

            ToName.Add("level", "");
            ToName.Add("head", "");
            ToName.Add("option", "");
            ToName.Add("addition", "");
            ToNameBool.Add("expireOnLogout", "登出消失");
            ToNameBool.Add("tradeBlock", "不可交易");
            ToNameBool.Add("notSale", "不可販賣");
            ToNameBool.Add("only", "持有專屬");
            ToNameBool.Add("equipTradeBlock", "裝備綁定");
            ToNameBool.Add("epicItem", "傳說");
            ToNameBool.Add("timeLimited", "限時物品");
            ToNameBool.Add("noExtend", "不可延長");
            ToNameBool.Add("notExtend", "不可延長");
            ToNameBool.Add("quest", "任務道具");
            ToNameBool.Add("accountSharable", "帳號移動");
            ToNameBool.Add("exItem", "套裝");
            ToNameBool.Add("randVariation", "randVariation");
            ToNameBool.Add("jokerToSetItem", "jokerToSetItem");

            ToName.Add("tradeAvailable", "");
            ToName.Add("cash", "");

            ToName.Add("mobcategory", "怪物剋星");
            ToName.Add("boss", "頭目剋星");
            ToName.Add("skill", "攻擊特效");
            ToName.Add("critical", "會心一擊");
            ToName.Add("mobdie", "獵殺特效");
            ToName.Add("statinc", "追加能力");
            var Dir = tabControl1.TabPages[tabIndex].Name;
            if (Arc.CharacterWz.GetNode(Dir) == null)
            {
                MessageBox.Show(Dir + "  not found");
                return;
            }

            Wz_Node Child = null;
            switch (Dir)
            {
                case "Totem": Child = Arc.StringWz.GetNode("Eqp.img/Eqp/Accessory"); break;
                case "TamingMob": Child = Arc.StringWz.GetNode("Eqp.img/Eqp/Taming"); break;
                default: Child = Arc.StringWz.GetNode("Eqp.img/Eqp/" + Dir); break;
            }
            var Row = -1;
            string ID, Desc, h1;
            Bitmap Icon = null;
            string IName, Data = "", D;
            foreach (var img in Arc.CharacterWz.GetNode(Dir).Nodes)
            {
                if (LeftStr(img.Text, 1) != "0")
                    continue;
                Row += 1;
                DumpData2(img.GetNode("info"));
                DumpData1();
                switch (Dir)
                {
                    case "Hair":
                        if (img.GetNode("default/hairOverHead") != null)
                            Icon = img.GetNode("default/hairOverHead").ExtractPng2();
                        break;
                    case "Face":
                        if (img.GetNode("default/face") != null)
                            Icon = img.GetNode("default/face").ExtractPng2();
                        break;
                    default:
                        if (img.GetNode("info/icon") != null)
                            Icon = img.GetNode("info/icon").ExtractPng2();
                        break;
                }
                ID = img.ImgID().IDString();
                ColList1.Add(GetTypes(ID));
                Desc = Child.GetValue2(ID + "/desc", "");
                h1 = Child.GetValue2(ID + "/h1", "");
                RowList[Row] += ", " + Desc + h1;

                Grid.Rows.Add(img.ImgID(), Icon, Child.GetValue2(ID + "/name", ""), "", RowList[Row]);
                foreach (var Iter in Arc.CharacterWz.GetNode(Dir + '/' + img.Text).Nodes)
                {
                    foreach (var Iter2 in Iter.Nodes)
                    {
                        if ((Iter.Text == "info") && (!(Iter2.Value is Wz_Png)))
                        {
                            if (ToName.ContainsKey(Iter2.Text))
                            {
                                IName = ToName[Iter2.Text];
                                Data = ToData(Iter2);
                                if ((Iter2.Text == "cash") && (Iter2.ValueToStr() == "0"))
                                    D = "";
                                else
                                    D = ", ";
                                if ((Data != "0") && (Data != ""))
                                    ColList1.Add(ToName[Iter2.Text] + Data + D);
                            }
                            else if (ToNameBool.ContainsKey(Iter2.Text))
                            {
                                ColList1.Add(ToNameBool[Iter2.Text] + ",");
                            }
                            else
                            {
                                IName = Iter2.Text;
                                if (Iter2.Value is string)
                                    Data = Iter2.Value.ToString();
                                if (Data == "1")
                                    D = "=" + Data + ",";
                                else
                                    D = ",";

                                if ((Iter2.Text != "afterImage") && (Iter2.Text != "islot") && (Iter2.Text != "vslot")
                                  && (Iter2.Text != "walk") && (Iter2.Text != "stand") && (Iter2.Text != "sfx") && (Iter2.Text
                                     != "attack") && (Iter2.Text != "setItemID"))
                                    ColList1.Add(Iter2.Text + D);
                            }

                            foreach (var Iter3 in Iter2.Nodes)
                            {
                                if (ToName.ContainsKey(Iter3.Text))
                                    ColList1.Add(ToName[Iter3.Text] + ", ");
                            }

                        }

                    }

                }

                if (Desc != "")
                    ColList1.Add(" " + Desc);
                if (h1 != "")
                    ColList1.Add(" " + h1);
            }

            if ((Dir == "TamingMob") && (Arc.Skill001Wz != null))
            {
                var Dict = new Dictionary<string, string>();
                for (int i = 11; i <= 28; i++)
                {
                    if (Arc.Skill001Wz.GetNode("8000" + i.ToString() + ".img") != null)
                    {
                        foreach (var Iter in Arc.Skill001Wz.GetNode("8000" + i.ToString() + ".img/skill").Nodes)
                        {
                            if ((Iter.GetNode("vehicleID") != null) && (!Dict.ContainsKey("0" + Iter.GetNode("vehicleID").Value.ToString())))
                                Dict.Add("0" + Iter.GetNode("vehicleID").Value.ToString(), Iter.Text);

                        }
                    }
                }
                for (int i = 0; i <= 9; i++)
                {
                    if (Arc.Skill001Wz.GetNode("80011" + i.ToString() + ".img") != null)
                    {
                        foreach (var Iter in Arc.Skill001Wz.GetNode("80011" + i.ToString() + ".img/skill").Nodes)
                        {
                            if ((Iter.GetNode("vehicleID") != null) && (!Dict.ContainsKey("0" + Iter.GetNode("vehicleID").Value.ToString())))
                                Dict.Add("0" + Iter.GetNode("vehicleID").Value.ToString(), Iter.Text);

                        }
                    }
                }
                for (int i = 0; i < Grid.RowCount - 1; i++)
                {
                    // if (Grid[0, i].Value is string)
                    {
                        var TamingID = Grid[0, i].Value.ToString();
                        // if ((Grid[2, i].Value == "") && (Dict.ContainsKey(TamingID)))
                        if (Dict.ContainsKey(TamingID))
                            if (Arc.StringWz.GetNode("Skill.img/" + Dict[TamingID]) != null)
                                Grid[2, i].Value = Arc.StringWz.GetNode("Skill.img/" + Dict[TamingID]).GetValue2("name", " ");
                    }

                }

            }

            PutGridData1(3);
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);


        }

        void LoadMap(int Part)
        {

            var Links = new List<(string, int)>();
            var MapNames = new Dictionary<string, (string, string)>();
            string StreetName, MapName;
            foreach (var Iter in Arc.StringWz.GetNode("Map.img").Nodes)
            {
                foreach (var Iter2 in Iter.Nodes)
                {
                    string ID = Iter2.Text.PadLeft(9, '0');
                    StreetName = Iter2.GetValue2("streetName", "");
                    MapName = Iter2.GetValue2("mapName", "");
                    if (!MapNames.ContainsKey(ID))
                        MapNames.Add(ID, (StreetName, MapName));
                }
            }
            Wz_Node MapDir;

            if (Arc.MapWz.GetNode("Map") != null)
                MapDir = Arc.MapWz.GetNode("Map");
            else
                MapDir = Arc.Map002Wz.GetNode("Map");
            Bitmap Icon; ;
            var Row = -1;

            foreach (var Dir in MapDir.Nodes)
            {
                if (LeftStr(Dir.Text, 3) != "Map")
                    continue;
                switch (Part)
                {
                    case 1:
                        if ((Dir.Text != "Map0") && (Dir.Text != "Map1") && (Dir.Text != "Map2") && (Dir.Text != "Map3"))
                            continue;
                        break;
                    case 2:
                        if ((Dir.Text != "Map4") && (Dir.Text != "Map5") && (Dir.Text != "Map6") && (Dir.Text != "Map7") && (Dir.Text != "Map8"))
                            continue;
                        break;
                    case 3:
                        if (Dir.Text != "Map9")
                            continue;
                        break;
                }

                foreach (var img in Dir.Nodes)
                {
                    Row += 1;
                    DumpData2(img.GetNode("info"));

                    if (MapNames.ContainsKey(img.ImgID()))
                    {
                        StreetName = MapNames[img.ImgID()].Item1;
                        MapName = MapNames[img.ImgID()].Item2;
                    }
                    else
                    {
                        StreetName = "";
                        MapName = "";
                    }

                    if (img.GetNode("miniMap/canvas") != null)
                        Icon = img.GetNode("miniMap/canvas").ExtractPng2();
                    else
                        Icon = null;

                    Grid.Rows.Add(img.ImgID(), Icon, StreetName, MapName, "");
                    var Link = img.GetNode("info/link");
                    if (Link != null)
                        Links.Add(("Map" + LeftStr(Link.Value.ToString(), 1) + "/" + Link.Value.ToString() + ".img", Row));
                }

            }

            for (int i = 0; i < Links.Count; i++)
            {
                var Child = MapDir.GetNode(Links[i].Item1 + "/miniMap/canvas");
                if (Child != null)
                    Grid[1, Links[i].Item2].Value = Child.ExtractPng2();
            }
            for (int i = 0; i < RowList.Count; i++)
            {
                Grid[4, i].Value = RowList[i];
            }
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);

        }

        string s1(string S)
        {
            switch (S[0].ToString())
            {
                case "L":
                    return "雷";
                    break;
                case "F":
                    return "火";
                    break;
                case "I":
                    return "冰";
                    break;
                case "S":
                    return "毒";
                    break;
                case "D":
                    return "暗";
                    break;
                case "P":
                    return "物理";
                    break;
                case "H":
                    return "聖";
                    break;
            }
            return null;
        }

        string s2(string S)
        {
            switch (S[1].ToString())
            {
                case "1":
                    return "免疫";
                    break;
                case "2":
                    return "抵抗";
                    break;
                case "3":
                    return "弱點";
                    break;
            }
            return null;
        }
        string Copy(string s, int index, int count)
        {
            if (index < 1)
                index = 1;
            if ((index > s.Length) || (count <= 0))
            {
                return "";
                // exit;
            }
            if (index + count - 1 > s.Length)
                count = s.Length - index + 1;
            return s.Substring(index - 1, count);
        }
        string ElemName(string S)
        {
            string A, D;
            int Num;
            Num = S.Length / 2;
            string Result = "";
            for (int i = 1; i <= Num; i++)
            {
                A = Copy(S, i * 2 - 1, 2);
                if (i < Num)
                    D = "、";
                else
                    D = "";
                Result = Result + s1(A) + s2(A) + D;
            }
            return Result;
        }
        string RightStr(string s, int count)
        {
            if (count > s.Length)
                count = s.Length;
            return s.Substring(s.Length - count, count);

        }
        void LoadMob(int Part)
        {

            var ToName = new Dictionary<string, string>();
            var Category = new Dictionary<string, string>();
            Category.Add("1", "動物型");
            Category.Add("2", "植物型");
            Category.Add("3", "魚類型");
            Category.Add("4", "爬蟲類型");
            Category.Add("5", "精靈型");
            Category.Add("6", "惡魔型");
            Category.Add("7", "不死型");
            Category.Add("8", "無機物型");

            ToName.Add("level", "等級:");
            ToName.Add("exp", "經驗值:");
            ToName.Add("maxMP", "MP:");
            ToName.Add("maxHP", "HP:");
            ToName.Add("speed", "速度:");
            ToName.Add("acc", "命中率:");
            ToName.Add("pushed", "KB:");
            ToName.Add("category", "分類:");
            ToName.Add("eva", "迴避率:");
            ToName.Add("link", "連結:");
            ToName.Add("elemAttr", "屬性:");
            ToName.Add("MADamage", "魔法攻擊:");
            ToName.Add("MDDamage", "魔法防禦:");
            ToName.Add("PADamage", "物理攻擊:");
            ToName.Add("PDDamage", "物理防禦:");
            ToName.Add("PDRate", "物理減傷:");
            ToName.Add("MDRate", "魔法減傷:");
            ToName.Add("boss", "Boss");
            ToName.Add("firstAttack", "主動攻擊");
            ToName.Add("charismaEXP", "領導經驗:");
            ToName.Add("hpRecovery", "每10秒HP回復:");
            ToName.Add("mpRecovery", "每10秒MP回復:");
            var Links = new List<(string, int)>();
            Wz_Node Child = null;
            Wz_Structure WzArchive = null;
            switch (Part)
            {
                case 1:
                    WzArchive = Arc.MobWz;
                    break;
                case 2:
                    WzArchive = Arc.Mob001Wz;
                    break;
                case 3:
                    WzArchive = Arc.Mob2Wz;
                    break;
            }
            var Row = -1;
            string Data = "", D = "";
            Bitmap Icon = null;
            foreach (var Iter in WzArchive.Nodes())
            {
                if (RightStr(Iter.Text, 4) != ".img")
                    continue;
                Row += 1;
                if (WzArchive.GetNode(Iter.Text + "/stand/0") != null)
                    Child = WzArchive.GetNode(Iter.Text + "/stand/0");
                else if (WzArchive.GetNode(Iter.Text + "/fly/0") != null)
                    Child = WzArchive.GetNode(Iter.Text + "/fly/0");
                DumpData1();
                DumpData2(WzArchive.GetNode(Iter.Text));
                if (Child != null)
                    Icon = Child.ExtractPng2();
                Grid.Rows.Add(Iter.ImgID(), Icon, Arc.StringWz.GetNode("Mob.img/" + Iter.ImgID().IDString()).GetValue2("name", ""), "");

                //return;
                var Link = Iter.GetNode("info/link");
                if (Link != null)
                    Links.Add((Link.Value.ToString() + ".img", Row));

                foreach (var Iter2 in WzArchive.GetNode(Iter.Text + "/info").Nodes)
                {
                    if ((Iter2.Text == "category") && (Category.ContainsKey(Iter2.ValueToStr())))
                        Data = Category[Iter2.ValueToStr()];

                    else if (Iter2.Text == "elemAttr")
                        Data = ElemName(Iter2.ValueToStr());

                    else if ((Iter2.Text == "boss") || (Iter2.Text == "firstAttack"))
                        Data = "";
                    else
                        Data = Iter2.ValueToStr();

                    if ((Iter2.Text == "PDRate") || (Iter2.Text == "MDRate"))
                        D = "%";
                    else
                        D = "";

                    if (ToName.ContainsKey(Iter2.Text))
                        // Grid.Cells[4 + Col, Row] := ToName[Iter2.Name] + Data + D + ',  ';
                        ColList1.Add(ToName[Iter2.Text] + Data + D + ",  ");

                }

            }

            for (int i = 0; i < Links.Count; i++)
            {
                if (WzArchive.GetNode(Links[i].Item1 + "/stand/0") != null)
                    Child = WzArchive.GetNode(Links[i].Item1 + "/stand/0");
                else if (WzArchive.GetNode(Links[i].Item1 + "/fly/0") != null)
                    Child = WzArchive.GetNode(Links[i].Item1 + "/fly/0");


                if (Arc.MobWz.GetNode(Links[i].Item1 + "/stand/0") != null)
                    Child = Arc.MobWz.GetNode(Links[i].Item1 + "/stand/0");
                else if (Arc.MobWz.GetNode(Links[i].Item1 + "/fly/0") != null)
                    Child = Arc.MobWz.GetNode(Links[i].Item1 + "/fly/0");


                Grid[1, Links[i].Item2].Value = Child.ExtractPng2();
            }
            for (int i = 0; i < RowList.Count; i++)
            {
                Grid[4, i].Value = RowList[i];
            }
            PutGridData1(3);
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
        }
        [DllImport("Eval2.dll", EntryPoint = "Eval", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void Eval(string Formula, ref double Value, ref int ErrPos);

        double GetFValue(string FormStr, int Level)
        {
            int E = 0;
            double A = 0;
            string Str = "";
            Str = FormStr.Replace("x", Level.ToString());
            Eval(Str, ref A, ref E);
            return A;
        }
        Wz_Node Common;
        int MaxLev;
        string CommonMatch(System.Text.RegularExpressions.Match Match1)
        {

            string MatchName = Copy(Match1.Value, 2, 100);
            foreach (var Iter in Common.Nodes)
            {
                if (Iter.Text == MatchName)
                    return GetFValue(Iter.Value.ToString(), MaxLev).ToString();
            }
            return null;
        }
        void LoadSkill(int Part)
        {
            Wz_Structure WzArchive = null;
            switch (Part)
            {
                case 1:
                    WzArchive = Arc.SkillWz;
                    break;
                case 2:
                    WzArchive = Arc.Skill001Wz;
                    break;
                case 3:
                    WzArchive = Arc.Skill002Wz;
                    break;
            }
            Bitmap Icon;
            foreach (var img in WzArchive.Nodes())
            {
                if (Char.IsNumber(img.Text, 0))
                {
                    DumpData2(WzArchive.GetNode(img.Text + "/info"));

                    var BookID = img.ImgID();
                    string BookName = "";
                    if (Arc.StringWz.GetNode("Skill.img/" + BookID) != null)
                        BookName = Arc.StringWz.GetNode("Skill.img/" + BookID).GetValue2("bookName", "");

                    if (WzArchive.GetNode(img.Text + "/info/icon") != null)
                        Icon = WzArchive.GetNode(img.Text + "/info/icon").ExtractPng2();
                    else
                        Icon = null;

                    Grid.Rows.Add(BookID, Icon, BookName, "", "");

                    foreach (var Iter in WzArchive.GetNode(img.Text).Nodes)
                    {
                        foreach (var Iter2 in Iter.Nodes)
                        {

                            if (Iter.Text == "skill")
                            {
                                DumpData2(Iter2);
                                var SkillID = Iter2.Text;
                                if (Iter2.GetNode("icon") != null)
                                    Icon = Iter2.GetNode("icon").ExtractPng2();
                                string SkillName = "", Desc = "";
                                if (Arc.StringWz.GetNode("Skill.img/" + SkillID) != null)
                                {
                                    SkillName = Arc.StringWz.GetNode("Skill.img/" + SkillID).GetValue2("name", "");
                                    Desc = Arc.StringWz.GetNode("Skill.img/" + SkillID).GetValue2("desc", "");
                                }
                                string hDesc = "";
                                var Child = Arc.StringWz.GetNode("Skill.img/" + SkillID);
                                if (Child != null)
                                {
                                    if (Child.GetNode("h") != null)
                                    {
                                        if (Child.GetNode("h").Value is string)
                                        {
                                            hDesc = Child.GetNode("h").Value.ToString();
                                        }
                                        hDesc = hDesc.Replace("mpConMP", "mpCon MP");
                                        hDesc = hDesc.Replace(",", " ,");
                                        Common = Iter2.GetNode("common");
                                        if (Common != null)
                                        {
                                            MaxLev = Common.GetValue2("maxLevel", 1);
                                            if (hDesc != "")
                                            {
                                                hDesc = "Lv." + MaxLev.ToString() + "= " + Regex.Replace(hDesc, "\\#[0-9,_,a-z,A-Z,\\.]+", CommonMatch);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 1; i <= 30; i++)
                                        {
                                            if (Child.GetNode("h" + i.ToString()) != null)
                                                hDesc = "Lv." + i.ToString() + "= " + Child.GetNode("h" + i.ToString()).Value.ToString();
                                        }
                                    }
                                }

                                Grid.Rows.Add(SkillID, Icon, SkillName, Desc, hDesc, "");
                            }
                        }
                    }
                }

            }
            for (int i = 0; i < RowList.Count; i++)
                Grid[5, i].Value = RowList[i];

            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
        }
        void LoadNpc()
        {
            var Row = -1;
            var Links = new List<(string, int)>();
            Bitmap Icon = null;
            foreach (var Img in Arc.NpcWz.Nodes())
            {
                Row += 1;
                var ID = Img.ImgID();
                var Entry = Arc.NpcWz.GetNode(Img.Text);
                if (Entry.GetNode("stand/0") != null)
                    Icon = Entry.GetNode("stand/0").ExtractPng2();
                if (Entry.GetNode("default/0") != null)
                    Icon = Entry.GetNode("default/0").ExtractPng2();
                var Name = Arc.StringWz.GetNode("Npc.img/" + ID.IDString()).GetValue2("name", "");
                DumpData2(Arc.StringWz.GetNode("Npc.img/" + ID.IDString()));
                Grid.Rows.Add(ID, Icon, Name, "");
                var Link = Arc.NpcWz.GetNode(Img.Text + "/info/link");
                if (Link != null)
                    Links.Add((Link.Value.ToString() + ".img", Row));
            }
            Wz_Node Child = null;
            for (int i = 0; i < Links.Count; i++)
            {
                if (Arc.NpcWz.GetNode(Links[i].Item1 + "/stand/0") != null)
                    Child = Arc.NpcWz.GetNode(Links[i].Item1 + "/stand/0");
                else if (Arc.NpcWz.GetNode(Links[i].Item1 + "/default/0") != null)
                    Child = Arc.NpcWz.GetNode(Links[i].Item1 + "/default/0");
                Grid[1, Links[i].Item2].Value = Child.ExtractPng2();
            }
            for (int i = 0; i < RowList.Count; i++)
                Grid[3, i].Value = RowList[i];
            ColList.Clear();
            RowList.Clear();
            foreach (var Img in Arc.NpcWz.Nodes())
            {
                DumpData2(Arc.NpcWz.GetNode(Img.Text + "/info"));
            }
            for (int i = 0; i < RowList.Count; i++)
            {
                Grid[4, i].Value = RowList[i];
            }
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);

        }
        void LoadMorph()
        {

            var Dict = new Dictionary<string, (string, string)>();
            var imgs = new List<string>();
            string Desc = "", Name = "";
            foreach (var Iter in Arc.StringWz.GetNode("Consume.img").Nodes)
            {
                Desc = Iter.GetValue2("desc", "");
                Name = Iter.GetValue2("name", "");
                Dict.Add(Iter.Text, (Name, Desc));
            }
            foreach (var img in Arc.MorphWz.Nodes())
            {
                imgs.Add(img.ImgID());
            }
            Bitmap Icon = null;
            Bitmap MorphPic = null;
            foreach (var Iter in Arc.ItemWz.GetNode("Consume/0221.img").Nodes)
            {
                DumpData2(Iter);
                var ID = Iter.Text;
                if (Dict.ContainsKey(Iter.Text.IDString()))
                {
                    Name = Dict[Iter.Text.IDString()].Item1;
                    Desc = Dict[Iter.Text.IDString()].Item2;
                }
                if (Iter.GetNode("info/icon") != null)
                    Icon = Iter.GetNode("info/icon").ExtractPng2();
                var MorphID = Iter.GetValue2("spec/morph", "").PadLeft(4, '0');
                if (imgs.Contains(MorphID))
                {
                    if (Arc.MorphWz.GetNode(MorphID + ".img/walk/0") != null)
                        MorphPic = Arc.MorphWz.GetNode(MorphID + ".img/walk/0").ExtractPng2();
                }
                Grid.Rows.Add(ID, Icon, MorphID, MorphPic, Name, Desc, "");
            }

            for (int i = 0; i < RowList.Count; i++)
                Grid[6, i].Value = RowList[i];
        }

        void LoadFamiliar()
        {

            if (Arc.CharacterWz.GetNode("Familiar") == null)
            {
                MessageBox.Show("Familiar  not found");
                return;
            }
            Wz_Node CardEntry;
            Bitmap MobPic = null, Icon = null;
            string CardID = "", MobID = "";
            foreach (var img in Arc.CharacterWz.GetNode("Familiar").Nodes)
            {
                var ID = img.ImgID();
                var Entry = Arc.CharacterWz.GetNode("Familiar/" + img.Text);
                if (Entry.GetNode("info/MobID") != null)
                    MobID = Entry.GetNode("info/MobID").Value.ToString().PadLeft(7, '0');
                else if (Arc.EtcWz.GetNode("FamiliarInfo.img") != null)
                    MobID = Arc.EtcWz.GetNode("FamiliarInfo.img/" + ID).GetValue2("mob", "100100").PadLeft(7, '0');

                DumpData2(Entry.GetNode("info"));

                if (Arc.MobWz.GetNode(MobID + ".img") != null)
                {
                    if (Arc.MobWz.GetNode(MobID + ".img/stand/0") != null)
                        MobPic = Arc.MobWz.GetNode(MobID + ".img/stand/0").ExtractPng2();
                    else if (Arc.MobWz.GetNode(MobID + ".img/fly/0") != null)
                        MobPic = Arc.MobWz.GetNode(MobID + ".img/fly/0").ExtractPng2();
                }
                else if ((Arc.Mob001Wz != null) && (Arc.Mob001Wz.GetNode(MobID + ".img") != null))
                {
                    if (Arc.Mob001Wz.GetNode(MobID + ".img/stand/0") != null)
                        MobPic = Arc.Mob001Wz.GetNode(MobID + ".img/stand/0").ExtractPng2();
                    else if (Arc.Mob001Wz.GetNode(MobID + ".img/fly/0") != null)
                        MobPic = Arc.Mob001Wz.GetNode(MobID + ".img/fly/0").ExtractPng2();
                }
                else if (Arc.Mob2Wz.GetNode(MobID + ".img") != null)
                {
                    if (Arc.Mob2Wz.GetNode(MobID + ".img/stand/0") != null)
                        MobPic = Arc.Mob2Wz.GetNode(MobID + ".img/stand/0").ExtractPng2();
                    else if (Arc.Mob2Wz.GetNode(MobID + ".img/fly/0") != null)
                        MobPic = Arc.Mob2Wz.GetNode(MobID + ".img/fly/0").ExtractPng2();
                }
                string SkillID = "";
                if (Entry.GetNode("info/skill") != null)
                    SkillID = Entry.GetNode("info/skill").GetValue2("id", "");
                string SkillName = "", SkillDesc = "";
                if (Arc.StringWz.GetNode("FamiliarSkill.img") != null)
                {
                    if (Arc.StringWz.GetNode("FamiliarSkill.img/skill/" + SkillID) != null)
                    {
                        SkillName = SkillID + ":" + Arc.StringWz.GetNode("FamiliarSkill.img/skill/" + SkillID).GetValue2("name", "");
                        SkillDesc = Arc.StringWz.GetNode("FamiliarSkill.img/skill/" + SkillID).GetValue2("desc", "");
                    }
                }

                else if (Arc.StringWz.GetNode("Familiar.img") != null)
                {
                    if (Arc.StringWz.GetNode("Familiar.img/skill/" + SkillID) != null)
                    {
                        SkillName = SkillID + ":" + Arc.StringWz.GetNode("Familiar.img/skill/" + SkillID).GetValue2("name", "");
                        SkillDesc = Arc.StringWz.GetNode("Familiar.img/skill/" + SkillID).GetValue2("desc", "");
                    }
                }
                if (Arc.EtcWz.GetNode("FamiliarInfo.img") != null)
                {
                    if (Arc.EtcWz.GetNode("FamiliarInfo.img/" + ID) != null)
                        CardID = Arc.EtcWz.GetNode("FamiliarInfo.img/" + ID).GetValue2("consume", "");
                }
                else
                {
                    if (Entry.GetNode("info/monsterCardID") != null)
                        CardID = Entry.GetNode("info/monsterCardID").Value.ToString();
                    else
                        CardID = "";
                }

                if (Arc.ItemWz.GetNode("Consume/0287.img/0" + CardID) != null)
                {
                    CardEntry = Arc.ItemWz.GetNode("Consume/0287.img/0" + CardID);
                    if (CardEntry.GetNode("info/icon") != null)
                        Icon = CardEntry.GetNode("info/icon").ExtractPng2();
                }
                else if (Arc.ItemWz.GetNode("Consume/0238.img/0" + CardID) != null)
                {
                    CardEntry = Arc.ItemWz.GetNode("Consume/0238.img/0" + CardID);
                    if (CardEntry.GetNode("info/iconRaw") != null)
                        Icon = CardEntry.GetNode("info/iconRaw").ExtractPng2();
                }
                else
                    Icon = null;

                var CardName = Arc.StringWz.GetNode("Consume.img/" + CardID).GetValue2("name", "");
                Grid.Rows.Add(ID, MobPic, "", SkillName, SkillDesc, CardID, Icon, CardName);
            }

            for (int i = 0; i < RowList.Count; i++)
                Grid[2, i].Value = RowList[i];
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
        }
        void LoadDamageSkin()
        {
            Bitmap Icon = null, Sample = null;
            foreach (var Iter in Arc.StringWz.GetNode("Consume.img").Nodes)
            {
                var Name = Iter.GetValue2("name", "");
                if ((Name.Contains("字型")) || (Name.Contains("ジスキン")) || (Name.Contains("스킨")) || (Name.Contains
                  ("Damage Skin")) || (Name.Contains("字型")) || (Name.Contains("伤害皮肤")))
                {
                    var ID = "0" + Iter.Text;
                    string[] imgs = new string[] { "0243.img", "0263.img" };
                    for (int i = 0; i <= 1; i++)
                    {
                        var Entry = Arc.ItemWz.GetNode("Consume/" + imgs[i] + "/0" + Iter.Text + "/info/icon");
                        if (Entry != null)
                            Icon = Entry.ExtractPng2();
                        Entry = Arc.ItemWz.GetNode("Consume/" + imgs[i] + "/0" + Iter.Text + "/info/sample");
                        if (Entry != null)
                            Sample = Entry.ExtractPng2();
                    }
                    var Desc = Iter.GetValue2("desc", "");
                    Grid.Rows.Add(ID, Icon, Name, Sample, Desc);
                }
            }
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
        }
        void LoadReactor()
        {
            var Row = -1;
            var Links = new List<(string, int)>();
            Bitmap Icon = null;

            foreach (var img in Arc.ReactorWz.Nodes())
            {
                Row += 1;
                DumpData2(Arc.ReactorWz.GetNode(img.Text + "/info"));
                var ID = img.ImgID();
                var Entry = Arc.ReactorWz.GetNode(img.Text + "/0/0");
                if (Entry != null)
                    Icon = Entry.ExtractPng2();
                Entry = Arc.ReactorWz.GetNode(img.Text + "/info/link");
                if (Entry != null)
                    Links.Add((Entry.Value.ToString(), Row));
                Grid.Rows.Add(ID, Icon, "");
            }

            Wz_Node Child = null;
            for (int i = 0; i < Links.Count; i++)
            {
                if (Arc.ReactorWz.GetNode(Links[i].Item1 + ".img/0/0") != null)
                    Child = Arc.ReactorWz.GetNode(Links[i].Item1 + ".img/0/0");
                Grid[1, Links[i].Item2].Value = Child.ExtractPng2();
            }
            for (int i = 0; i < RowList.Count; i++)
            {
                Grid[2, i].Value = RowList[i];
            }
            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);

        }
        void LoadMusic()
        {
            foreach (var img in Arc.SoundWz.Nodes())
            {
                if (LeftStr(img.Text, 3) != "Bgm")
                    continue;
                foreach (var Iter in  Arc.SoundWz.GetNode(img.Text).Nodes )
                {
                   Grid.Rows.Add(Iter.GetPath());
                }
               
            }

            if (Arc.Sound2Wz != null)
            {
                foreach (var img in Arc.Sound2Wz.Nodes())
                {
                    if (LeftStr(img.Text, 3) == "Bgm" || LeftStr(img.Text, 4) == "PL_3" || LeftStr(img.Text, 4) == "PL_B" || LeftStr(img.Text, 4) == "PL_C" || LeftStr(img.Text, 4) == "PL_M")
                    {

                        foreach (var Iter in Arc.Sound2Wz.GetNode(img.Text).Nodes)
                        {
                            if(Iter.Value is Wz_Sound)
                             Grid.Rows.Add(Iter.GetPath());
                        }
                    }

                }
            }

            Grid.Sort(Grid.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
        }
        DataViewer[] DataGrid = new DataViewer[40];
        DataViewer[] TempGrid = new DataViewer[40];
        bool FirstLoadBIN = false;
        void LoadBIN()
        {
            var BinFile = System.Environment.CurrentDirectory + "\\" + Grid.Parent.Name + ".BIN";
            if (System.IO.File.Exists(BinFile))
            {
                for (int i = 0; i <= 38; i++)
                {
                    DataGrid[i].Rows.Clear();
                    DataGrid[i].Refresh();
                    var Graphic = DataGrid[i].CreateGraphics();
                    var Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold);
                    Graphic.DrawString("Loading...", Font, Brushes.Black, 300, 200);
                }
                Grid.LoadBin(BinFile);
                if (!FirstLoadBIN)
                {
                    MessageBox.Show("提示: 必須指定楓之谷資料夾路徑才會顯示道具 ToolTip UI");
                    FirstLoadBIN = true;
                }
            }
            else
                MessageBox.Show(Grid.Parent.Name + ".BIN" + " not found");
        }



        public Form1()
        {


            InitializeComponent();
        }
        Wz_Node find(string c)
        {
            return null;
        }
        ToolTip ToolTip2;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Load From BIN")
            {
                label1.Visible = false;
                SelectFolderBox.Visible = false;
                LoadButton.Visible = false;
                LoadBIN();
            }
            else
            {
                label1.Visible = true;
                SelectFolderBox.Visible = true;
                LoadButton.Visible = true;
                Grid.Rows.Clear();
                Grid.Refresh();
            }

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (this.mapRenderGame2 != null)
            {
                this.mapRenderGame2.form.Visible = false;
                //  this.Focus();
            }

            Row1 = -1;
            tabIndex = tabControl1.SelectedIndex;
            Grid = DataGrid[tabIndex];
            SearchGrid = TempGrid[tabIndex];
            Grid.Visible = true;
            SearchGrid.Visible = false;
            SearchBox.Clear();
            comboBox4.SelectedIndex = tabControl1.SelectedIndex;
            switch (Grid.DefaultGridType)
            {
                case GridType.Normal:
                case GridType.Item:
                    Grid.RowTemplate.Height = 40;
                    break;
                case GridType.Map:
                    Grid.RowTemplate.Height = 60;
                    break;

                case GridType.Mob:
                case GridType.Reactor:
                    Grid.RowTemplate.Height = 80;
                    break;

                case GridType.Skill:
                    Grid.RowTemplate.Height = 60;
                    break;

                case GridType.Npc:
                case GridType.Morph:
                case GridType.Familiar:
                    Grid.RowTemplate.Height = 70;
                    break;

                case GridType.DamageSkin:
                    Grid.RowTemplate.Height = 50;
                    break;
            }

            if (comboBox1.Text == "Load From BIN")
                LoadBIN();
            ToolTip2.tooltipQuickView.Visible = false;
            SetGrid();


        }
        string GetWzFileName()
        {

            switch (tabIndex)
            {
                case 0:
                case 1:
                case 26:
                case 27:
                case 36:
                case 37:
                    return "Item.wz";
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 35:
                    return "Character.wz";
                    break;
                case 16:
                case 17:
                case 18:
                    return "Map.wz";
                    break;
                case 19:
                    return "Mob.wz";
                    break;
                case 20:
                    return "Mob001.wz";
                    break;
                case 21:
                    return "Mob2.wz";
                    break;
                case 22:
                    return "Skill.wz";
                    break;
                case 23:
                    return "Skill001.wz";
                    break;
                case 24:
                    return "Skill002.wz";
                    break;
                case 25:
                    return "Npc.wz";
                    break;
                case 34:
                    return "Morph.wz";
                    break;
                case 38:
                    return "Reactor.wz";
                    break;
                case 39:
                    return "Sound.wz";
                    break;
            }
            return "";
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(SelectFolderBox.Text + "\\" + GetWzFileName()))
            {
                MessageBox.Show("Wrong MapleStory folder,  " + GetWzFileName() + "  couble not be found");
                return;
            }
            Row1 = -1;
            Grid.Rows.Clear();
            Grid.Refresh();
            RowList.Clear();
            ColList.Clear();
            var Graphic = Grid.CreateGraphics();
            var Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold);
            Graphic.DrawString("Loading...", Font, Brushes.Black, 300, 200);
            switch (tabIndex)
            {
                case 0:
                case 1:
                case 26:
                case 27:
                case 37:
                    LoadItem();
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                    LoadCharacter();
                    break;
                case 16:
                    LoadMap(1);
                    break;

                case 17:
                    LoadMap(2);
                    break;
                case 18:
                    LoadMap(3);
                    break;
                case 19:
                    LoadMob(1);
                    break;
                case 20:
                    LoadMob(2);
                    break;
                case 21:
                    LoadMob(3);
                    break;
                case 22:
                    LoadSkill(1);
                    break;
                case 23:
                    LoadSkill(2);
                    break;
                case 24:
                    LoadSkill(3);
                    break;
                case 25:
                    LoadNpc();
                    break;
                case 34:
                    LoadMorph();
                    break;
                case 35:
                    LoadFamiliar();
                    break;
                case 36:
                    LoadDamageSkin();
                    break;
                case 38:
                    LoadReactor();
                    break;

                case 39:
                    LoadMusic();
                    break;

            }

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Grid.SaveBin(System.Environment.CurrentDirectory + "\\" + Grid.Parent.Name + ".BIN");
            MessageBox.Show("儲存 " + Grid.Parent.Name + ".BIN 完成");
        }
        string Trim(string s)
        {

            return s.Trim(' ');
        }
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var SearchStr = Trim(SearchBox.Text);
            if (SearchStr == "")
            {
                Grid.Visible = true;
                SearchGrid.Visible = false;
            }
            else
            {
                SearchGrid.Rows.Clear();
                var Row = new DataGridViewRow();
                for (int i = 0; i < Grid.RowCount; i++)
                {
                    for (int j = 0; j < Grid.Columns.Count; j++)
                    {
                        if (Grid.Rows[i].Cells[j].Value is string)
                        {
                            if (Grid.Rows[i].Cells[j].Value.ToString().IndexOf(SearchStr, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Row = (DataGridViewRow)Grid.Rows[i].Clone();
                                for (int j2 = 0; j2 < Grid.Columns.Count; j2++)
                                    Row.Cells[j2].Value = Grid.Rows[i].Cells[j2].Value;
                                SearchGrid.Rows.Add(Row);
                                break;
                            }
                        }
                    }
                }
                Grid.Visible = false;
                SearchGrid.Visible = true;
                SearchGrid.Refresh();
            }

        }
        bool FileExists(string Name)
        {
            return System.IO.File.Exists(Name);

        }

        private EventHandler<WzStructureEventArgs> wzOpened;
        protected virtual void OnWzOpened(WzStructureEventArgs e)
        {
            if (wzOpened != null)
            {
                wzOpened(this, e);
            }
        }

        BassSoundPlayer soundPlayer;
        private void SelectFolderBox_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog().ToString() == "OK")
            {

                SelectFolderBox.Text = folderBrowserDialog1.SelectedPath;
                var FolderPath = SelectFolderBox.Text;
                if (FileExists(FolderPath + "\\string.wz"))
                {

                    //  Wz_Structure.DefaultEncoding = Encoding.Default;
                    // Wz_Structure.DefaultAutoDetectExtFiles = false;
                      Wz_Structure.DefaultImgCheckDisabled =true;
                    Arc.MobWz = new Wz_Structure();
                    Arc.MapWz = new Wz_Structure();
                    Arc.SkillWz = new Wz_Structure();
                    Arc.ItemWz = new Wz_Structure();
                    Arc.CharacterWz = new Wz_Structure();
                    Arc.NpcWz = new Wz_Structure();
                    Arc.MorphWz = new Wz_Structure();
                    Arc.StringWz = new Wz_Structure();
                    Arc.EtcWz = new Wz_Structure();
                    Arc.ReactorWz = new Wz_Structure();
                    Arc.UIWz = new Wz_Structure();
                    Arc.SoundWz = new Wz_Structure();
                    Arc.EffectWz = new Wz_Structure();
                    Arc.BaseWz = new Wz_Structure();

                    Arc.MobWz.Load(FolderPath + "\\Mob.wz");
                    if (FileExists(FolderPath + "\\Mob001.wz"))
                    {
                        Arc.Mob001Wz = new Wz_Structure();
                        Arc.Mob001Wz.Load(FolderPath + "\\Mob001.wz");
                        ToolTip2.openedWz.Add(Arc.Mob001Wz);
                    }

                    if (FileExists(FolderPath + "\\Mob2.wz"))
                    {
                        Arc.Mob2Wz = new Wz_Structure();
                        Arc.Mob2Wz.Load(FolderPath + "\\Mob2.wz");
                        ToolTip2.openedWz.Add(Arc.Mob2Wz);
                    }

                    Arc.MapWz.Load(FolderPath + "\\Map.wz");
                    if (FileExists(FolderPath + "\\Map001.wz"))
                    {
                        Arc.Map001Wz = new Wz_Structure();
                        Arc.Map001Wz.Load(FolderPath + "\\Map001.wz");
                        ToolTip2.openedWz.Add(Arc.Map001Wz);
                    }
                    if (FileExists(FolderPath + "\\Map002.wz"))
                    {
                        Arc.Map002Wz = new Wz_Structure();
                        Arc.Map002Wz.Load(FolderPath + "\\Map002.wz");
                        ToolTip2.openedWz.Add(Arc.Map002Wz);
                    }
                    if (FileExists(FolderPath + "\\Map2.wz"))
                    {
                        Arc.Map2Wz = new Wz_Structure();
                        Arc.Map2Wz.Load(FolderPath + "\\Map2.wz");
                        ToolTip2.openedWz.Add(Arc.Map2Wz);
                    }

                    Arc.SkillWz.Load(FolderPath + "\\Skill.wz");
                    if (FileExists(FolderPath + "\\Skill001.wz"))
                    {
                        Arc.Skill001Wz = new Wz_Structure();
                        Arc.Skill001Wz.Load(FolderPath + "\\Skill001.wz");
                        ToolTip2.openedWz.Add(Arc.Skill001Wz);
                    }
                    if (FileExists(FolderPath + "\\Skill002.wz"))
                    {
                        Arc.Skill002Wz = new Wz_Structure();
                        Arc.Skill002Wz.Load(FolderPath + "\\Skill002.wz");
                        ToolTip2.openedWz.Add(Arc.Skill002Wz);
                    }
                    if (FileExists(FolderPath + "\\Sound2.wz"))
                    {
                        Arc.Sound2Wz = new Wz_Structure();
                        Arc.Sound2Wz.Load(FolderPath + "\\Sound2.wz");
                        ToolTip2.openedWz.Add(Arc.Sound2Wz);
                    }
                    if (FileExists(FolderPath + "\\Sound001.wz"))
                    {
                        Arc.Sound001Wz = new Wz_Structure();
                        Arc.Sound001Wz.Load(FolderPath + "\\Sound001.wz");
                        // ToolTip2.openedWz.Add(Arc.Sound001Wz);
                    }
                    Arc.ItemWz.Load(FolderPath + "\\Item.wz");
                    Arc.CharacterWz.Load(FolderPath + "\\Character.wz");
                    Arc.NpcWz.Load(FolderPath + "\\Npc.wz");
                    Arc.MorphWz.Load(FolderPath + "\\Morph.wz");
                    Arc.StringWz.Load(FolderPath + "\\String.wz");
                    Arc.EtcWz.Load(FolderPath + "\\Etc.wz");
                    Arc.ReactorWz.Load(FolderPath + "\\Reactor.wz");
                    Arc.UIWz.Load(FolderPath + "\\UI.wz");
                    Arc.SoundWz.Load(FolderPath + "\\Sound.wz");
                    Arc.EffectWz.Load(FolderPath + "\\Effect.wz");
                    Arc.BaseWz.Load(FolderPath + "\\Base.wz");

                    ToolTip2.openedWz.Add(Arc.CharacterWz);
                    ToolTip2.openedWz.Add(Arc.ItemWz);
                    ToolTip2.openedWz.Add(Arc.SkillWz);
                    ToolTip2.openedWz.Add(Arc.UIWz);
                    ToolTip2.openedWz.Add(Arc.EtcWz);
                    ToolTip2.openedWz.Add(Arc.ReactorWz);
                    ToolTip2.openedWz.Add(Arc.EffectWz);
                    ToolTip2.openedWz.Add(Arc.MobWz);
                    ToolTip2.openedWz.Add(Arc.StringWz);
                    ToolTip2.openedWz.Add(Arc.NpcWz);
                    ToolTip2.openedWz.Add(Arc.MapWz);
                    ToolTip2.openedWz.Add(Arc.SoundWz);

                    //  ToolTip2.openedWz.Add(Arc.BaseWz);


                }

            }
        }


        // Wz_Structure s;
        private void button1_Click(object sender, EventArgs e)
        {




        }
        string GetIDPath(string ID)
        {


            switch (tabIndex)
            {
                case 0:
                    return "Item/Cash/" + LeftStr(ID, 4) + ".img/" + ID;
                    break;
                case 1:
                case 34:
                case 36:
                    return "Item/Consume/" + LeftStr(ID, 4) + ".img/" + ID;
                    break;
                case 2:
                    return "Character/Weapon/" + ID + ".img";
                    break;
                case 3:
                    return "Character/Cap/" + ID + ".img";
                    break;
                case 4:
                    return "Character/Coat/" + ID + ".img";
                    break;
                case 5:
                    return "Character/Longcoat/" + ID + ".img";
                    break;

                case 6:
                    return "Character/Pants/" + ID + ".img";
                    break;
                case 7:
                    return "Character/Shoes/" + ID + ".img";
                    break;
                case 8:
                    return "Character/Glove/" + ID + ".img";
                    break;
                case 9:
                    return "Character/Ring/" + ID + ".img";
                    break;

                case 10:
                    return "Character/Cape/" + ID + ".img";
                    break;

                case 11:
                    return "Character/Accessory/" + ID + ".img";
                    break;
                case 12:
                    return "Character/Shield/" + ID + ".img";
                    break;
                case 13:
                    return "Character/TamingMob/" + ID + ".img";
                    break;
                case 14:
                    return "Character/Hair/" + ID + ".img";
                    break;

                case 15:
                    return "Character/Face/" + ID + ".img";
                    break;
                case 19:
                case 20:
                case 21:
                    return "Mob/" + ID + ".img";
                    break;

                case 22:
                    if (LeftStr(ID, 1) == "0")
                        return "Skill/000.img/skill/" + ID;
                    else
                        return "Skill/" + (int.Parse(ID) / 10000).ToString() + ".img/skill/" + ID;
                    break;


                case 23:
                    if (LeftStr(ID, 1) == "8")
                        return "Skill/" + (int.Parse(ID) / 100).ToString() + ".img/skill/" + ID;
                    else
                        return "Skill/" + (int.Parse(ID) / 10000).ToString() + ".img/skill/" + ID;
                    break;

                case 24:
                    return "Skill/" + (int.Parse(ID) / 10000).ToString() + ".img/skill/" + ID;
                    break;

                case 25:
                    return "Npc/" + ID + ".img";
                    break;
                case 26:
                    return "Item/Pet/" + ID + ".img";
                    break;

                case 27:
                    if (Arc.ItemWz == null) return null;
                    if (Arc.ItemWz.GetNode("Install/03010.img") != null)
                    {
                        switch (LeftStr(ID, 5))
                        {
                            case "03015":
                                return "Item/Install/" + LeftStr(ID, 6) + ".img/" + ID;
                                break;
                            case "03010":
                            case "03011":
                            case "03012":
                            case "03013":
                            case "03014":
                            case "03016":
                            case "03017":
                            case "03018":
                                return "Item/Install/" + LeftStr(ID, 5) + ".img/" + ID;
                                break;
                            default:
                                return "Item/Install/" + LeftStr(ID, 4) + ".img/" + ID;
                                break;
                        }
                    }
                    else
                    {
                        return "Item/Install/" + LeftStr(ID, 4) + ".img/" + ID;
                    }
                    break;

                case 28:
                    return "Character/Android/" + ID + ".img";
                    break;
                case 29:
                    return "Character/Mechanic/" + ID + ".img";
                    break;

                case 30:
                    return "Character/PetEquip/" + ID + ".img";
                    break;

                case 31:
                    return "Character/Bits/" + ID + ".img";
                    break;

                case 32:
                    return "Character/MonsterBattle/" + ID + ".img";
                    break;

                case 33:
                    return "Character/Totem/" + ID + ".img";
                    break;

                case 37:
                    return "Item/Etc/" + LeftStr(ID, 4) + ".img/" + ID;
                    break;
            }
            /*
            switch (LeftStr(ID, 2))
            {
                case "05":
                    return "Item/Cash/" + LeftStr(ID, 4) + ".img/" + ID;
                    //return "Item/Cash/0501.img/05010000";
                    break;
                case "03":
                    if (Arc.ItemWz.GetNode("Install/03010.img") != null)
                    {
                        switch (LeftStr(ID, 5))
                        {
                            case "03015":
                                return "Item/Install/" + LeftStr(ID, 6) + ".img/" + ID;
                                break;
                            case "03010":
                            case "03011":
                            case "03012":
                            case "03013":
                            case "03014":
                            case "03016":
                            case "03017":
                            case "03018":
                                return "Item/Install/" + LeftStr(ID, 5) + ".img/" + ID;
                                break;
                            default:
                                return "Item/Install/" + LeftStr(ID, 4) + ".img/" + ID;
                                break;
                        }
                    }
                    else
                    {
                        return "Item/Install/" + LeftStr(ID, 4) + ".img/" + ID;
                    }
                    break;



            }
            switch (int.Parse(ID) / 10000)
            {
                case 2:
                    return "Character/Face/" + ID + ".img";
                    break;
                case 3:
                case 4:
                case 6:
                    return "Character/Hair/" + ID + ".img";
                    break;
                case 101:
                case 102:
                case 103:
                case 112:
                case 113:
                case 114:
                case 115:
                case 116:
                case 118:
                case 119:
                    return "Character/Accessory/" + ID + ".img";
                    break;
                case 120:
                    return "Character/Totem/" + ID + ".img";
                    break;
                case 100:
                    return "Character/Cap/" + ID + ".img";
                    break;
                case 110:
                    return "Character/Cape/" + ID + ".img";
                    break;
                case 104:
                    return "Character/Coat/" + ID + ".img";
                    break;
                case 105:
                    return "Character/Longcoat/" + ID + ".img";
                    break;
                case 106:
                    return "Character/Pants/" + ID + ".img";
                    break;
                case 107:
                    return "Character/Shoes/" + ID + ".img";
                    break;
                case 108:
                    return "Character/Glove/" + ID + ".img";
                case 109:
                    return "Character/Shield/" + ID + ".img"; 
                    break;

                case 111:
                    return "Character/Ring/" + ID + ".img";
                    break;

                case 161:
                    return "Character/Mechanic/" + ID + ".img";
                    break;

                case 166:
                case 167:
                    return "Character/Android/" + ID + ".img";
                    break;
                case 168:
                    return "Character/Bits/" + ID + ".img";
                    break;
                case int n when (n >=121 && n <=170):
                    return "Character/Weapon/" + ID + ".img";
                    break;
                case int n when (n >= 190 && n <= 199):
                    return "Character/TamingMob/" + ID + ".img";
                    break;
                case 180:
                    return "Character/PetEquip/" + ID + ".img";
                    break;
                case 996:
                case 997:
                    return "Character/Familiar/" + ID + ".img";
                    break;
                case int n when (n >= 200 && n <= 294):
                    return  "Item/Consume/" + LeftStr(ID, 4) + ".img/" + ID;
                    break;
                case int n when (n >= 400 && n <= 446):
                    return "Item/Etc/" + LeftStr(ID, 4) + ".img/" + ID;
                    break;

                case 500:
                    return "Item/Pet/" + ID + ".img";
                    break;


            }
            */
            return null;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Grid.DefaultCellStyle.Font = new System.Drawing.Font("微軟正黑體", int.Parse(comboBox2.Text));
            SearchGrid.DefaultCellStyle.Font = new System.Drawing.Font("微軟正黑體", int.Parse(comboBox2.Text));
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Grid.RowTemplate.Height = int.Parse(comboBox3.Text);
            //  SearchGrid.RowTemplate.Height = int.Parse(comboBox3.Text);
            LoadButton_Click(sender, e);


        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

            tabControl1.SelectedIndex = comboBox4.SelectedIndex;

        }

     

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                ToolTip2.tooltipQuickView.Visible = false;

        }
        FrmMapRender2 mapRenderGame2;
        void ShowMap(Wz_Node MapImg)
        {
            Wz_Node node = MapImg;
            if (node != null)
            {
                Wz_Image img = (Wz_Image)node.Value;
                Wz_File wzFile = node.GetNodeWzFile();

                if (img != null && img.TryExtract())
                {
                    if (wzFile == null || wzFile.Type != Wz_Type.Map)
                    {
                        if (MessageBox.Show("所选Img不属于Map.wz，是否继续？", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        {
                            Text = "nonono";  // goto exit;
                        }
                    }
                    StringLinker sl;
                    sl = new StringLinker();
                    sl.Load(PluginManager.FindWz(Wz_Type.String).GetValueEx<Wz_File>(null));
                    if (this.mapRenderGame2 != null)
                    {
                      
                        this.mapRenderGame2.Dispose();
                        this.mapRenderGame2 = null;
                    }
                   

                    this.mapRenderGame2 = new FrmMapRender2(img) { StringLinker = sl };
                    this.mapRenderGame2.Window.Title = "MapRender ";
                    this.mapRenderGame2.Run();
                }
            }
        }

                  
        void SetGrid()
        {
            //  Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid.DefaultCellStyle.SelectionBackColor = Color.LightCyan;
            Grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            foreach (DataGridViewColumn column in Grid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            Grid.EnableHeadersVisualStyles = false;
            Grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Grid.ColumnHeadersDefaultCellStyle.BackColor;
            Grid.RowHeadersVisible = false;
            Grid.Dock = DockStyle.Fill;
            Grid.ShowCellToolTips = false;
            //
            //SearchGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            SearchGrid.DefaultCellStyle.SelectionBackColor = Color.LightCyan;
            SearchGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            foreach (DataGridViewColumn column in SearchGrid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            SearchGrid.EnableHeadersVisualStyles = false;
            SearchGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Grid.ColumnHeadersDefaultCellStyle.BackColor;
            SearchGrid.RowHeadersVisible = false;
            SearchGrid.Dock = DockStyle.Fill;
            SearchGrid.ShowCellToolTips = false;


            Grid.CellClick += (s, e) =>
            {
                if (e.RowIndex == -1) return;
                if (e.RowIndex >= Grid.RowCount) return;
                if (tabIndex == 38 || tabIndex== 35 ) return;
                string SelectID = "";
                if (Grid.Rows[e.RowIndex].Cells[0].Value is string)
                {
                    SelectID = Grid.Rows[e.RowIndex].Cells[0].Value.ToString();

                    if (tabIndex == 16 || tabIndex == 17 || tabIndex == 18)
                    {
                        Wz_Node imgNode=null;
                        if(Arc.Map002Wz!= null &&  Arc.Map002Wz.GetNode("Map")!=null)
                           imgNode = Arc.Map002Wz.WzNode.FindNodeByPath("Map\\Map" + LeftStr(SelectID, 1) + "\\" + SelectID + ".img");
                        else if(Arc.MapWz.GetNode("Map")!=null)
                            imgNode = Arc.MapWz.WzNode.FindNodeByPath("Map\\Map" + LeftStr(SelectID, 1) + "\\" + SelectID + ".img");
                        ShowMap(imgNode);
                      
                    }
                    else if(tabIndex==39)
                    {

                        Wz_Sound sound = null; 
                        if (Arc.SoundWz.GetNode(SelectID)!=null)
                           sound= (Wz_Sound)Arc.SoundWz.GetNode(SelectID).Value;
                        else if(Arc.Sound2Wz!=null && Arc.Sound2Wz.GetNode(SelectID) != null)
                          sound = (Wz_Sound)Arc.Sound2Wz.GetNode(SelectID).Value;

                        soundPlayer.UnLoad();
                        byte[] data = sound.ExtractSound();
                        if (data == null || data.Length <= 0)
                        {
                            return;
                        }
                        soundPlayer.PreLoad(data);
                        soundPlayer.Play();
                    }
                    else
                    {
                        if (tabIndex == 39) return;
                        var Node = PluginManager.FindWz(GetIDPath(SelectID));
                        ToolTip2.quickView(Node);
                        ToolTip2.tooltipQuickView.Visible = true;
                    }

                }
            };
            Grid.Scroll += (s, e) =>
            {
                ToolTip2.tooltipQuickView.Visible = false;
                if (this.mapRenderGame2 != null)
                {
                   this.mapRenderGame2.form.Visible=false;
                   //  this.Focus();
                }
            };
            SearchGrid.CellClick += (s, e) =>
            {
                if (e.RowIndex == -1) return;
                if (e.RowIndex >= SearchGrid.RowCount) return;
                if (tabIndex == 38 || tabIndex == 35 || tabIndex==39) return;
                string SelectID = "";
                if (SearchGrid.Rows[e.RowIndex].Cells[0].Value is string)
                {
                    SelectID = SearchGrid.Rows[e.RowIndex].Cells[0].Value.ToString();

                    if (tabIndex == 16 || tabIndex == 17 || tabIndex == 18)
                    {
                        Wz_Node imgNode = null;
                        if (Arc.Map002Wz != null && Arc.Map002Wz.GetNode("Map") != null)
                            imgNode = Arc.Map002Wz.WzNode.FindNodeByPath("Map\\Map" + LeftStr(SelectID, 1) + "\\" + SelectID + ".img");
                        else if (Arc.MapWz.GetNode("Map") != null)
                            imgNode = Arc.MapWz.WzNode.FindNodeByPath("Map\\Map" + LeftStr(SelectID, 1) + "\\" + SelectID + ".img");
                        ShowMap(imgNode);

                    }
                    else
                    {
                        var Node = PluginManager.FindWz(GetIDPath(SelectID));
                        ToolTip2.quickView(Node);
                        ToolTip2.tooltipQuickView.Visible = true;
                    }

                }
            };
            SearchGrid.Scroll += (s, e) =>
            {
                ToolTip2.tooltipQuickView.Visible = false;
                if (this.mapRenderGame2 != null)
                {
                    this.mapRenderGame2.form.Visible = false;
                    //  this.Focus();
                }
            };
            Grid.DefaultCellStyle.Font = new System.Drawing.Font("微軟正黑體", int.Parse(comboBox2.Text));
            SearchGrid.DefaultCellStyle.Font = new System.Drawing.Font("微軟正黑體", int.Parse(comboBox2.Text));

            Grid.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    ContextMenuStrip m = new ContextMenuStrip();
                    m.Items.Add("Copy");
                    int currentMouseOverRow = Grid.HitTest(e.X, e.Y).RowIndex;
                    m.Show(Grid, new Point(e.X, e.Y));
                    if (Grid.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        try
                        {
                            // Add the selection to the clipboard.
                            Clipboard.SetDataObject(Grid.GetClipboardContent());
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            MessageBox.Show("The Clipboard could not be accessed. Please try again.");
                        }
                    }
                }

            };
            //
            SearchGrid.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    ContextMenuStrip m = new ContextMenuStrip();
                    m.Items.Add("Copy");
                    int currentMouseOverRow = SearchGrid.HitTest(e.X, e.Y).RowIndex;
                    m.Show(SearchGrid, new Point(e.X, e.Y));
                    if (SearchGrid.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        try
                        {
                            // Add the selection to the clipboard.
                            Clipboard.SetDataObject(SearchGrid.GetClipboardContent());
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            MessageBox.Show("The Clipboard could not be accessed. Please try again.");
                        }
                    }
                }

            };

        }
        private void Form1_Load(object sender, EventArgs e)
        {

           
            ColList = new List<string>();
            RowList = new List<string>();
            RowList1 = new Dictionary<int, List<string>>();
            soundPlayer = new BassSoundPlayer();
            if (!soundPlayer.Init())
            {
                Un4seen.Bass.BASSError error = soundPlayer.GetLastError();
                MessageBox.Show("Bass初始化失败！\r\n\r\nerrorCode : " + (int)error + "(" + error + ")", "虫子");
            }
            for (int i = 0; i <= 39; i++)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                    case 26:
                    case 27:
                    case 37:
                        DataGrid[i] = new DataViewer(GridType.Item);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Item);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;

                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                        DataGrid[i] = new DataViewer(GridType.Normal);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Normal);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;

                    case 16:
                    case 17:
                    case 18:
                        DataGrid[i] = new DataViewer(GridType.Map);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Map);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;
                    case 19:
                    case 20:
                    case 21:
                        DataGrid[i] = new DataViewer(GridType.Mob);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Mob);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;
                    case 22:
                    case 23:
                    case 24:
                        DataGrid[i] = new DataViewer(GridType.Skill);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Skill);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;
                    case 25:
                        DataGrid[i] = new DataViewer(GridType.Npc);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Npc);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;
                    case 34:
                        DataGrid[i] = new DataViewer(GridType.Morph);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Morph);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;
                    case 35:
                        DataGrid[i] = new DataViewer(GridType.Familiar);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Familiar);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;
                    case 36:
                        DataGrid[i] = new DataViewer(GridType.DamageSkin);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.DamageSkin);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;
                    case 38:
                        DataGrid[i] = new DataViewer(GridType.Reactor);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Reactor);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;

                    case 39:
                        DataGrid[i] = new DataViewer(GridType.Music);
                        DataGrid[i].Parent = tabControl1.TabPages[i];
                        TempGrid[i] = new DataViewer(GridType.Music);
                        TempGrid[i].Parent = tabControl1.TabPages[i];
                        break;



                }



            }

            Grid = DataGrid[0];

            SearchGrid = TempGrid[0];
            ToolTip2 = new ToolTip();
            SetGrid();
            Grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            Graphics graphics = this.CreateGraphics();
            float dpiX = graphics.DpiX;
            double Size10 = ((double)96 / (double)dpiX) * 10;
            double Size11 = ((double)96 / (double)dpiX) * 11;
            double Size12 = ((double)96 / (double)dpiX) * 12;
            double Size13 = ((double)96 / (double)dpiX) * 13;

            comboBox1.Font = new Font("微軟正黑體", (float)Size10);
            comboBox2.Font = new Font("微軟正黑體", (float)Size10);
            comboBox3.Font = new Font("微軟正黑體", (float)Size10);
            comboBox4.Font = new Font("微軟正黑體", (float)Size12);
            label1.Font = new Font("微軟正黑體", (float)Size12);
            label2.Font = new Font("微軟正黑體", (float)Size12);
            label3.Font = new Font("微軟正黑體", (float)Size12);
            label4.Font = new Font("微軟正黑體", (float)Size12);
            label6.Font = new Font("微軟正黑體", (float)Size12);
            SelectFolderBox.Font = new Font("微軟正黑體", (float)Size11);
            SearchBox.Font = new Font("微軟正黑體", (float)Size11);
            LoadButton.Font = new Font("微軟正黑體", (float)Size12);
            SaveButton.Font = new Font("微軟正黑體", (float)Size12);
            tabControl1.Font = new Font("微軟正黑體", (float)Size13);

        }




    }

    public class ToolTip
    {

        public ToolTip()
        {

            openedWz = new List<Wz_Structure>();
            stringLinker = new StringLinker();
            PluginManager.WzFileFinding += new FindWzEventHandler(CharaSimLoader_WzFileFinding);


            tooltipQuickView = new AfrmTooltip();
            tooltipQuickView.Visible = true;
            tooltipQuickView.StringLinker = this.stringLinker;
            tooltipQuickView.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            // tooltipQuickView.KeyDown += new KeyEventHandler(afrm_KeyDown);
            tooltipQuickView.ShowID = true;
            tooltipQuickView.ShowMenu = true;
            tooltipQuickView.Left = 700;
            tooltipQuickView.Top = 150;
            // tooltipQuickView.GearRender.ShowMedalTag = true;
        }


        EventHandler<WzStructureEventArgs> wzOpened2;
        // event EventHandler<WzStructureEventArgs> PluginContextProvider.WzOpened
        // {
        //  add { wzOpened += value; }
        // remove { wzOpened -= value; }
        // }


        public List<Wz_Structure> openedWz;
        DefaultLevel skillDefaultLevel = DefaultLevel.Level0;
        int skillInterval = 32;
        void CharaSimLoader_WzFileFinding(object sender, FindWzEventArgs e)
        {

            string[] fullPath = null;
            if (!string.IsNullOrEmpty(e.FullPath)) //用fullpath作为输入参数
            {
                fullPath = e.FullPath.Split('/', '\\');
                try
                {
                    e.WzType = (Wz_Type)Enum.Parse(typeof(Wz_Type), fullPath[0], true);
                }
                catch
                {
                    e.WzType = Wz_Type.Unknown;
                }
            }

            List<Wz_Node> preSearch = new List<Wz_Node>();
            if (e.WzType != Wz_Type.Unknown) //用wztype作为输入参数
            {
                IEnumerable<Wz_Structure> preSearchWz = e.WzFile?.WzStructure != null ?
                    Enumerable.Repeat(e.WzFile.WzStructure, 1) :
                    this.openedWz;
                foreach (var wzs in preSearchWz)
                {
                    Wz_File baseWz = null;
                    bool find = false;
                    foreach (Wz_File wz_f in wzs.wz_files)
                    {
                        if (wz_f.Type == e.WzType)
                        {
                            preSearch.Add(wz_f.Node);
                            find = true;
                            //e.WzFile = wz_f;
                        }
                        if (wz_f.Type == Wz_Type.Base)
                        {
                            baseWz = wz_f;
                        }
                    }

                    // detect data.wz
                    if (baseWz != null && !find)
                    {
                        string key = e.WzType.ToString();
                        foreach (Wz_Node node in baseWz.Node.Nodes)
                        {
                            if (node.Text == key && node.Nodes.Count > 0)
                            {
                                preSearch.Add(node);
                            }
                        }
                    }
                }
            }

            if (fullPath == null || fullPath.Length <= 1)
            {
                if (e.WzType != Wz_Type.Unknown && preSearch.Count > 0) //返回wzFile
                {
                    e.WzNode = preSearch[0];
                    e.WzFile = preSearch[0].Value as Wz_File;
                }
                return;
            }

            if (preSearch.Count <= 0)
            {
                return;
            }

            foreach (var wzFileNode in preSearch)
            {
                var searchNode = wzFileNode;
                for (int i = 1; i < fullPath.Length && searchNode != null; i++)
                {
                    searchNode = searchNode.Nodes[fullPath[i]];
                    var img = searchNode.GetValueEx<Wz_Image>(null);
                    if (img != null)
                    {
                        searchNode = img.TryExtract() ? img.Node : null;
                    }
                }

                if (searchNode != null)
                {
                    e.WzNode = searchNode;
                    e.WzFile = wzFileNode.Value as Wz_File;
                    return;
                }
            }
            //寻找失败
            e.WzNode = null;
        }
        public AfrmTooltip tooltipQuickView;
        CharaSimControlGroup charaSimCtrl;
        StringLinker stringLinker;

        private Wz_File findStringWz()
        {
            foreach (Wz_Structure wz in openedWz)
            {
                foreach (Wz_File file in wz.wz_files)
                {
                    if (file.Type == Wz_Type.String)
                    {
                        return file;
                    }
                }
            }
            return null;
        }
        public void quickView(Wz_Node node)
        {

            Wz_Node selectedNode = node;
            if (selectedNode == null)
            {
                return;
            }

            Wz_Image image;

            Wz_File wzf = selectedNode.GetNodeWzFile();
            if (wzf == null)
            {


                //  labelItemStatus.Text = "没有查询到节点所属的wzfile。";
                return;
            }

            if (!this.stringLinker.HasValues)
            {
                this.stringLinker.Load(findStringWz());
            }

            object obj = null;
            string fileName = null;
            switch (wzf.Type)
            {
                case Wz_Type.Character:
                    if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                        return;
                    CharaSimLoader.LoadSetItemsIfEmpty();
                    CharaSimLoader.LoadExclusiveEquipsIfEmpty();
                    var gear = Gear.CreateFromNode(image.Node, PluginManager.FindWz);
                    obj = gear;
                    if (gear != null)
                    {
                        fileName = gear.ItemID + ".png";
                    }
                    break;
                case Wz_Type.Item:
                    Wz_Node itemNode = selectedNode;
                    if (Regex.IsMatch(itemNode.FullPathToFile, @"^Item\\(Cash|Consume|Etc|Install|Cash)\\\d{4,6}.img\\\d+$"))
                    {
                        var item = Item.CreateFromNode(itemNode, PluginManager.FindWz);
                        obj = item;
                        if (item != null)
                        {
                            fileName = item.ItemID + ".png";
                        }
                    }
                    else if (Regex.IsMatch(itemNode.FullPathToFile, @"^Item\\Pet\\\d{7}.img"))
                    {
                        if (CharaSimLoader.LoadedSetItems.Count == 0) //宠物 预读套装
                        {
                            CharaSimLoader.LoadSetItemsIfEmpty();
                        }
                        if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                            return;
                        var item = Item.CreateFromNode(image.Node, PluginManager.FindWz);
                        obj = item;
                        if (item != null)
                        {
                            fileName = item.ItemID + ".png";
                        }
                    }

                    break;
                case Wz_Type.Skill:
                    Wz_Node skillNode = selectedNode;
                    //模式路径分析
                    if (Regex.IsMatch(skillNode.FullPathToFile, @"^Skill\d*\\Recipe_\d+.img\\\d+$"))
                    {
                        Recipe recipe = Recipe.CreateFromNode(skillNode);
                        obj = recipe;
                        if (recipe != null)
                        {
                            fileName = "recipe_" + recipe.RecipeID + ".png";
                        }
                    }
                    else if (Regex.IsMatch(skillNode.FullPathToFile, @"^Skill\d*\\\d+.img\\skill\\\d+$"))
                    {
                        Skill skill = Skill.CreateFromNode(skillNode, PluginManager.FindWz);
                        if (skill != null)
                        {
                            switch (this.skillDefaultLevel)
                            {
                                case DefaultLevel.Level0: skill.Level = 0; break;
                                case DefaultLevel.Level1: skill.Level = 1; break;
                                case DefaultLevel.LevelMax: skill.Level = skill.MaxLevel; break;
                                case DefaultLevel.LevelMaxWithCO: skill.Level = skill.MaxLevel + 2; break;
                            }
                            obj = skill;
                            fileName = "skill_" + skill.SkillID + ".png";
                        }
                    }
                    break;

                case Wz_Type.Mob:
                    // this.Text = "gd";
                    if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                        return;
                    var mob = Mob.CreateFromNode(image.Node, PluginManager.FindWz);
                    obj = mob;
                    if (mob != null)
                    {
                        fileName = mob.ID + ".png";
                    }
                    break;

                case Wz_Type.Npc:
                    if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                        return;
                    var npc = Npc.CreateFromNode(image.Node, PluginManager.FindWz);
                    obj = npc;
                    if (npc != null)
                    {
                        fileName = npc.ID + ".png";
                    }
                    break;
            }
            if (obj != null)
            {
                tooltipQuickView.TargetItem = obj;
                tooltipQuickView.ImageFileName = fileName;
                tooltipQuickView.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
                tooltipQuickView.Refresh();
                tooltipQuickView.HideOnHover = false;
                tooltipQuickView.Show();
            }
        }

    }


}






