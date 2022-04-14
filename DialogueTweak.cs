using DialogueTweak.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DialogueTweak
{
    public class DialogueTweak : Mod
    {
        // ����Icon��List����һЩĬ��ֵ
        internal static List<IconInfo> IconInfos = new List<IconInfo>() {
            new IconInfo(IconType.Shop, NPCID.Guide, "DialogueTweak/Interfaces/Buttons/Icon_Help"),
            new IconInfo(IconType.Extra, NPCID.Guide, "DialogueTweak/Interfaces/Buttons/Icon_Hammer"),
            new IconInfo(IconType.Shop, NPCID.OldMan, "DialogueTweak/Interfaces/Buttons/Icon_Old_Man"),
            new IconInfo(IconType.Shop, NPCID.TaxCollector, "Head"),
            new IconInfo(IconType.Shop, NPCID.Angler, "Head"),
            new IconInfo(IconType.Shop, NPCID.Nurse, "Head"),
        };
        public static Texture2D DefaultIcon;
        public static Texture2D SignIcon;
        public static Texture2D EditIcon;
        public override void PostSetupContent() {
            if (Main.netMode != NetmodeID.Server) {
                ButtonHandler.Button_Back = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Button_Back");
                ButtonHandler.Button_BackLong = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Button_BackLong");
                ButtonHandler.Button_Happiness = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Button_Happiness");
                ButtonHandler.Button_Highlight = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Button_Highlight");

                ButtonHandler.ButtonLong = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/ButtonLong");
                ButtonHandler.ButtonLong_Highlight = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/ButtonLong_Highlight");
                ButtonHandler.ButtonLonger = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/ButtonLonger");
                ButtonHandler.ButtonLonger_Highlight = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/ButtonLonger_Highlight");

                ButtonHandler.Shop = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Icon_Default");
                ButtonHandler.Extra = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Icon_Default");

                DefaultIcon = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Icon_Default");
                SignIcon = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Icon_Sign");
                EditIcon = ModContent.GetTexture("DialogueTweak/Interfaces/Buttons/Icon_Edit");

                GUIChat.GreyPixel = ModContent.GetTexture("DialogueTweak/Interfaces/GreyPixel");
                GUIChat.PortraitPanel = ModContent.GetTexture("DialogueTweak/Interfaces/PortraitPanel");
                GUIChat.ChatStringBack = ModContent.GetTexture("DialogueTweak/Interfaces/ChatStringBack");
            }
        }

        internal static DialogueTweak instance;
        internal static string prevText;
        internal static float letterAppeared;

        public override void Load() {
            base.Load();
            instance = this;
            On.Terraria.Main.GUIChatDrawInner += Main_GUIChatDrawInner;
        }

        public override void Unload() {
            instance = null;
        }

        // ͨ������screenWidthʹһ�л��Ƶ���Ļ֮�⣬NPC�Ի����Ʋ��ᱻӰ��
        private void Main_GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self) {
            // ȷ���Ǵ���NPC�Ի�״̬��PC���б༭��ʾ��ʲô��Ҳ�����UI��
            GUIChat.GUIDrawInner();
        }

        public override void UpdateUI(GameTime gameTime) {
            if (Main.npcChatText != prevText) {
                letterAppeared = 0;
            }
            prevText = Main.npcChatText;
            if (!Main.npc.IndexInRange(Main.LocalPlayer.talkNPC) || Main.npc[Main.LocalPlayer.talkNPC] is null || !Main.npc[Main.LocalPlayer.talkNPC].active) {
                return;
            }
            if (Main.LocalPlayer.sign > -1) {
                letterAppeared = 1145141919; // ����û�л������ֻ���
                return;
            }
            if (letterAppeared < Main.npcChatText.Length) {
                float speakingRateMultipiler = GameCulture.Chinese.IsActive ? 1 : 1.6f;
                letterAppeared += ChatMethods.HandleSpeakingRate(Main.npc[Main.LocalPlayer.talkNPC].type) * speakingRateMultipiler;
            }
        }

        public override object Call(params object[] args) {
            try {
                if (args is null) {
                    throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
                }

                if (args.Length == 0) {
                    throw new ArgumentException("Arguments cannot be empty!");
                }

                if (args[0] is string msg) {
                    switch (msg) {
                        case "ReplaceExtraButtonIcon":
                            if (args.Length <= 3) {
                                IconInfos.Add(new IconInfo(
                                    IconType.Extra, // This icon is for extra button.
                                    Convert.ToInt32(args[1]), // NPC ID
                                    args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
                                    ));
                            }
                            else {
                                IconInfos.Add(new IconInfo(
                                    IconType.Extra, // This icon is for extra button.
                                    Convert.ToInt32(args[1]), // NPC ID
                                    args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
                                    args[3] as Func<bool> // Available
                                    ));
                            }
                            return true;
                        case "ReplaceShopButtonIcon":
                            if (args.Length <= 3) {
                                IconInfos.Add(new IconInfo(
                                    IconType.Shop, // This icon is for shop button.
                                    Convert.ToInt32(args[1]), // NPC ID
                                    args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
                                    ));
                            }
                            else {
                                IconInfos.Add(new IconInfo(
                                    IconType.Shop, // This icon is for shop button.
                                    Convert.ToInt32(args[1]), // NPC ID
                                    args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
                                    args[3] as Func<bool> // Available
                                ));
                            }
                            return true;
                        default:
                            Logger.Error($"Replacement type \"{msg}\" not found.");
                            return false;
                    }
                }
            }
            catch (Exception e) {
                Logger.Error($"{e.StackTrace} {e.Message}");
            }

            return false;
        }
    }
}