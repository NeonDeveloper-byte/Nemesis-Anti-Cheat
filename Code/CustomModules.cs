using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.Network.Serialization;
using LabFusion.Player;
using LabFusion.Representation;
using LabFusion.SDK.Gamemodes;
using LabFusion.SDK.Modules;
using LabFusion.SDK.Points;
using LabFusion.UI.Popups;
using LabFusion.Utilities;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static FusionProtectorByJamesReborn.Main;

namespace FusionProtectorByJamesReborn
{
    public class SendBase64FileData : INetSerializable
    {

        public byte smallId;
        public string Link;
        public string FileName;

        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref Link);
            serializer.SerializeValue(ref FileName);
        }

        public static SendBase64FileData Create(
            byte smallId,
            string kink,
            string filename)
        {
            return new SendBase64FileData()
            {
                smallId = smallId,
                Link = kink,
                FileName = filename
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class SendBase64FileMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 3f;
        public static float _timeOfRequest = -1000f;
        public static string codemodname = "";
        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.base64files)
                return;

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;

            var data = received.ReadData<SendBase64FileData>();

            if (data.smallId != received.Sender.Value)
                return;

            NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var playernow);


            var stringplayername = Main.CleanedNAME(playernow);


            if (playernow != null)
            {
                Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"{stringplayername} Sent You [{data.Link} - A Code Mod {data.FileName}] Accept To GET!", NotificationType.SUCCESS, 4.0f, true, true, () =>
                {


                    MelonCoroutines.Start(SiteStuff.ReadFromSite(
            data.Link,
            (sitetext) =>
            {
                if (File.Exists(Path.Combine(MelonEnvironment.ModsDirectory, $"{data.FileName}.dll")))
                {
                    NotificationNowAlways(FusionProtectorInfo.ClientName, $"File {data.FileName}.dll EXIST IN YOUR MOD FOLDER ALREADY CAN'T DO THIS!", NotificationType.ERROR, 3.5f);
                    return;
                }


                byte[] fileBytes = Convert.FromBase64String(sitetext);
                File.WriteAllBytes(Path.Combine(MelonEnvironment.ModsDirectory, $"{data.FileName}.dll"), fileBytes);
                NotificationNowAlways(FusionProtectorInfo.ClientName, $"Installed {data.FileName}.dll To Your Mods Folder Restart The Game For Mod To LOAD!", NotificationType.SUCCESS,3.5f);
            }));


                });
            }
        }
    }
    public class ShareDevToolPresetData : INetSerializable
    {
        public byte smallId;

        internal string TitleOfPreset;

        internal string Slot1Barcode;
        internal int Slot1ModID;
        internal string Slot1BarCodeName;
        internal bool Slot1LocalSpawn;

        internal string Slot2Barcode;
        internal int Slot2ModID;
        internal string Slot2BarCodeName;
        internal bool Slot2LocalSpawn;

        internal string Slot3Barcode;
        internal int Slot3ModID;
        internal string Slot3BarCodeName;
        internal bool Slot3LocalSpawn;

        internal string Slot4Barcode;
        internal int Slot4ModID;
        internal string Slot4BarCodeName;
        internal bool Slot4LocalSpawn;

        internal string Slot5Barcode;
        internal int Slot5ModID;
        internal string Slot5BarCodeName;
        internal bool Slot5LocalSpawn;

        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref TitleOfPreset);

            serializer.SerializeValue(ref Slot1Barcode);
            serializer.SerializeValue(ref Slot1ModID);
            serializer.SerializeValue(ref Slot1BarCodeName);
            serializer.SerializeValue(ref Slot1LocalSpawn);

            serializer.SerializeValue(ref Slot2Barcode);
            serializer.SerializeValue(ref Slot2ModID);
            serializer.SerializeValue(ref Slot2BarCodeName);
            serializer.SerializeValue(ref Slot2LocalSpawn);

            serializer.SerializeValue(ref Slot3Barcode);
            serializer.SerializeValue(ref Slot3ModID);
            serializer.SerializeValue(ref Slot3BarCodeName);
            serializer.SerializeValue(ref Slot3LocalSpawn);

            serializer.SerializeValue(ref Slot4Barcode);
            serializer.SerializeValue(ref Slot4ModID);
            serializer.SerializeValue(ref Slot4BarCodeName);
            serializer.SerializeValue(ref Slot4LocalSpawn);

            serializer.SerializeValue(ref Slot5Barcode);
            serializer.SerializeValue(ref Slot5ModID);
            serializer.SerializeValue(ref Slot5BarCodeName);
            serializer.SerializeValue(ref Slot5LocalSpawn);
        }

        public static ShareDevToolPresetData Create(
            byte smallId,
            string titleOfPreset,

            string slot1Barcode, int slot1ModID, string slot1Name, bool slot1LocalSpawn,
            string slot2Barcode, int slot2ModID, string slot2Name, bool slot2LocalSpawn,
            string slot3Barcode, int slot3ModID, string slot3Name, bool slot3LocalSpawn,
            string slot4Barcode, int slot4ModID, string slot4Name, bool slot4LocalSpawn,
            string slot5Barcode, int slot5ModID, string slot5Name, bool slot5LocalSpawn)
        {
            return new ShareDevToolPresetData()
            {
                smallId = smallId,
                TitleOfPreset = titleOfPreset,

                Slot1Barcode = slot1Barcode,
                Slot1ModID = slot1ModID,
                Slot1BarCodeName = slot1Name,
                Slot1LocalSpawn = slot1LocalSpawn,

                Slot2Barcode = slot2Barcode,
                Slot2ModID = slot2ModID,
                Slot2BarCodeName = slot2Name,
                Slot2LocalSpawn = slot2LocalSpawn,

                Slot3Barcode = slot3Barcode,
                Slot3ModID = slot3ModID,
                Slot3BarCodeName = slot3Name,
                Slot3LocalSpawn = slot3LocalSpawn,

                Slot4Barcode = slot4Barcode,
                Slot4ModID = slot4ModID,
                Slot4BarCodeName = slot4Name,
                Slot4LocalSpawn = slot4LocalSpawn,

                Slot5Barcode = slot5Barcode,
                Slot5ModID = slot5ModID,
                Slot5BarCodeName = slot5Name,
                Slot5LocalSpawn = slot5LocalSpawn
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class ShareDevToolPresetMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 3f;
        public static float _timeOfRequest = -1000f;
        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.sharedevtoolpresets)
                return;

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;

            var data = received.ReadData<ShareDevToolPresetData>();

            if (data.smallId != received.Sender.Value)
                return;

            NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var playernow);


            var stringplayername = Main.CleanedNAME(playernow);


            if (playernow != null)
            {
                Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"{stringplayername} Sent You [{data.TitleOfPreset} - DevToolPreset] Accept To GET!", NotificationType.SUCCESS, 4.0f, true, true, () =>
                {

                    if (!Main.IsBarcodeInGame(data.Slot1Barcode))
                    {
                        Main.DownloadModIOMod(data.Slot1ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot2Barcode))
                    {
                        Main.DownloadModIOMod(data.Slot2ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot3Barcode))
                    {
                        Main.DownloadModIOMod(data.Slot3ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot4Barcode))
                    {
                        Main.DownloadModIOMod(data.Slot4ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot5Barcode))
                    {
                        Main.DownloadModIOMod(data.Slot5ModID, false);
                    }
                 


                    CreateDevToolPreset(data.TitleOfPreset, new CreateCheatToolsPreset.Item
                    {
                        SpawnableName = data.Slot1BarCodeName,
                        BarcodeId = data.Slot1Barcode,
                        LocalSpawn = data.Slot1LocalSpawn,
                        ModIoID = data.Slot1ModID
                    }, new CreateCheatToolsPreset.Item
                    {
                        SpawnableName = data.Slot2BarCodeName,
                        BarcodeId = data.Slot2Barcode,
                        LocalSpawn = data.Slot2LocalSpawn,
                        ModIoID = data.Slot2ModID
                    }, new CreateCheatToolsPreset.Item
                    {
                        SpawnableName = data.Slot3BarCodeName,
                        BarcodeId = data.Slot3Barcode,
                        LocalSpawn = data.Slot3LocalSpawn,
                        ModIoID = data.Slot3ModID
                    }, new CreateCheatToolsPreset.Item
                    {
                        SpawnableName = data.Slot4BarCodeName,
                        BarcodeId = data.Slot4Barcode,
                        LocalSpawn = data.Slot4LocalSpawn,
                        ModIoID = data.Slot4ModID
                    }, new CreateCheatToolsPreset.Item
                    {
                        SpawnableName = data.Slot5BarCodeName,
                        BarcodeId = data.Slot5Barcode,
                        LocalSpawn = data.Slot5LocalSpawn,
                        ModIoID = data.Slot5ModID
                    });
                });
            } 
        }
    }
    public class ShareBodyLogPageData : INetSerializable
    {

        public byte smallId;

        internal string TitleOfPreset;
        internal string Slot1;
        internal string Slot2;
        internal string Slot3;
        internal string Slot4;
        internal string Slot5;
        internal string Slot6;

        internal int _modIoID1;
        internal int _modIoID2;
        internal int _modIoID3;
        internal int _modIoID4;
        internal int _modIoID5;
        internal int _modIoID6;


        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref TitleOfPreset);
            serializer.SerializeValue(ref Slot1);
            serializer.SerializeValue(ref Slot2);
            serializer.SerializeValue(ref Slot3);
            serializer.SerializeValue(ref Slot4);
            serializer.SerializeValue(ref Slot5);
            serializer.SerializeValue(ref Slot6);

            serializer.SerializeValue(ref _modIoID1);
            serializer.SerializeValue(ref _modIoID2);
            serializer.SerializeValue(ref _modIoID3);
            serializer.SerializeValue(ref _modIoID4);
            serializer.SerializeValue(ref _modIoID5);
            serializer.SerializeValue(ref _modIoID6);
        }

        public static ShareBodyLogPageData Create(byte smallId,string titleOfPreset,string slot1,string slot2,string slot3,string slot4,string slot5,string slot6,int modIoID1,int modIoID2,int modIoID3,int modIoID4,int modIoID5,int modIoID6)
        {
            return new ShareBodyLogPageData()
            {
                smallId = smallId,
                TitleOfPreset = titleOfPreset,
                Slot1 = slot1,
                Slot2 = slot2,
                Slot3 = slot3,
                Slot4 = slot4,
                Slot5 = slot5,
                Slot6 = slot6,
                _modIoID1 = modIoID1,
                _modIoID2 = modIoID2,
                _modIoID3 = modIoID3,
                _modIoID4 = modIoID4,
                _modIoID5 = modIoID5,
                _modIoID6 = modIoID6
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class ShareBodyLogPageMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 3f;
        public static float _timeOfRequest = -1000f;
        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.sharebodylogpagenow)
                return;

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;

            var data = received.ReadData<ShareBodyLogPageData>();

            if (data.smallId != received.Sender.Value)
                return;

            NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var playernow);


            var stringplayername = Main.CleanedNAME(playernow);


            if (playernow != null)
            {
                Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"{stringplayername} Sent You [{data.TitleOfPreset} - Bodylog Page] Accept To GET!", NotificationType.SUCCESS, 4.0f, true, true, () =>
                {

                    if (!Main.IsBarcodeInGame(data.Slot1))
                    {
                        Main.DownloadModIOMod(data._modIoID1, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot2))
                    {
                        Main.DownloadModIOMod(data._modIoID2, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot3))
                    {
                        Main.DownloadModIOMod(data._modIoID3, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot4))
                    {
                        Main.DownloadModIOMod(data._modIoID4, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot5))
                    {
                        Main.DownloadModIOMod(data._modIoID5, false);
                    }
                    if (!Main.IsBarcodeInGame(data.Slot6))
                    {
                        Main.DownloadModIOMod(data._modIoID6, false);
                    }





                    bool exists = false;

                    foreach (var t in BodyLogPage.BodyLogPages)
                    {
                        if (string.Equals(t.TitleOfPreset, data.TitleOfPreset, StringComparison.OrdinalIgnoreCase))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        BodyLogPage.BodyLogPages.Add(new BodyLogPage(data.TitleOfPreset, data.Slot1,
        data.Slot2,
        data.Slot3,
        data.Slot4,
        data.Slot5,
        data.Slot6,
        data._modIoID1,
        data._modIoID2,
        data._modIoID3,
        data._modIoID4,
        data._modIoID5,
        data._modIoID6));
                        BodyLogPage.SavePresets();
                        NotificationNow(FusionProtectorInfo.ClientName,
        $"Added Preset {data.TitleOfPreset}!",
        NotificationType.SUCCESS,
        2.5f);
                    }
                    else
                    {
                        NotificationNow(FusionProtectorInfo.ClientName,
                            "This Preset Name Exists Already!",
                            NotificationType.WARNING,
                            2.5f);
                    }
                });

            }

        }
    }
    public class SendBitData : INetSerializable
    {
        public byte smallId;
        public int bits;

        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref bits);
        }

        public static SendBitData Create(byte smallId, int bits)
        {
            return new SendBitData()
            {
                smallId = smallId,
                bits = bits,
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class SendBitMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 3f;
        public static float _timeOfRequest = -1000f;
        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.bitsending)
                return;

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;

            var data = received.ReadData<SendBitData>();

            if (data.smallId != received.Sender.Value)
                return;

            NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var playernow);


            var stringplayername = Main.CleanedNAME(playernow);


            if (playernow != null)
            {
                Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"{stringplayername} Sent You [{data.bits}] Bits Accept To GET!", NotificationType.SUCCESS, 4.0f, true, true, () =>
                {
                     PointItemManager.RewardBits(data.bits);
                });
            }

        }
    }
    public class SendModIDData : INetSerializable
    {
        public byte smallId;
        public int modid;

        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref modid);
        }

        public static SendModIDData Create(byte smallId, int modcid)
        {
            return new SendModIDData()
            {
                smallId = smallId,
                modid = modcid,
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class SendModIDMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 5f;
        public static float _timeOfRequest = -1000f;
        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.modidsending)
                return;

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;

            var data = received.ReadData<SendModIDData>();

            if (data.smallId != received.Sender.Value)
                return;

            NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var playernow);


            var stringplayername = Main.CleanedNAME(playernow);


            if (playernow != null)
            {
                Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"{stringplayername} Sent A Mod IO Mod For You To Download #{data.modid}!", NotificationType.SUCCESS, 4.0f, true, true, () =>
                {
                   
                    Main.DownloadModIOMod(data.modid);
              
                    Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"Downloading Mod.IO Mod From {stringplayername}", NotificationType.WARNING, 3.5f);
                });
            }

        }
    }
    public class SendBodyLogData : INetSerializable
    {   
        public byte smallId;
        public string BodyLogAvatar1;
        public string BodyLogAvatar2;
        public string BodyLogAvatar3;
        public string BodyLogAvatar4;
        public string BodyLogAvatar5;
        public string BodyLogAvatar6;


        public int BodyLogAvatar1ModID;
        public int BodyLogAvatar2ModID;
        public int BodyLogAvatar3ModID;
        public int BodyLogAvatar4ModID;
        public int BodyLogAvatar5ModID;
        public int BodyLogAvatar6ModID;

        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref BodyLogAvatar1);
            serializer.SerializeValue(ref BodyLogAvatar2);
            serializer.SerializeValue(ref BodyLogAvatar3);
            serializer.SerializeValue(ref BodyLogAvatar4);
            serializer.SerializeValue(ref BodyLogAvatar5);
            serializer.SerializeValue(ref BodyLogAvatar6);
            serializer.SerializeValue(ref BodyLogAvatar1ModID);
            serializer.SerializeValue(ref BodyLogAvatar2ModID);
            serializer.SerializeValue(ref BodyLogAvatar3ModID);
            serializer.SerializeValue(ref BodyLogAvatar4ModID);
            serializer.SerializeValue(ref BodyLogAvatar5ModID);
            serializer.SerializeValue(ref BodyLogAvatar6ModID);
        }

        public static SendBodyLogData Create(byte smallId,string slot1,string slot2,string slot3,string slot4,string slot5,string slot6, int slot1id,int slot2id,int slot3id,int slot4id,int slot5id,int slot6id)
        {
            return new SendBodyLogData()
            {
                smallId = smallId,
                BodyLogAvatar1 = slot1,
                BodyLogAvatar2 = slot2,
                BodyLogAvatar3 = slot3,
                BodyLogAvatar4 = slot4,
                BodyLogAvatar5 = slot5,
                BodyLogAvatar6 = slot6,
                BodyLogAvatar1ModID = slot1id,
                BodyLogAvatar2ModID = slot2id,
                BodyLogAvatar3ModID = slot3id,
                BodyLogAvatar4ModID = slot4id,
                BodyLogAvatar5ModID = slot5id,
                BodyLogAvatar6ModID = slot6id
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class SendBodyLogMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 5f;
        public static float _timeOfRequest = -1000f;
        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.bodylogsending)
                return;

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;
            var data = received.ReadData<SendBodyLogData>();

            if (data.smallId != received.Sender.Value)
                return;


            NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var playernow);


            var stringplayername = Main.CleanedNAME(playernow);


            if (playernow !=null)
            {
                Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"{stringplayername} Sent There Bodylog To You!", NotificationType.SUCCESS, 4.0f, true, true, () =>
                {
                    if (!Main.IsBarcodeInGame(data.BodyLogAvatar1))
                    {
                        Main.DownloadModIOMod(data.BodyLogAvatar1ModID,false);
                    }
                    if (!Main.IsBarcodeInGame(data.BodyLogAvatar2))
                    {
                        Main.DownloadModIOMod(data.BodyLogAvatar2ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.BodyLogAvatar3))
                    {
                        Main.DownloadModIOMod(data.BodyLogAvatar3ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.BodyLogAvatar4))
                    {
                        Main.DownloadModIOMod(data.BodyLogAvatar4ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.BodyLogAvatar5))
                    {
                        Main.DownloadModIOMod(data.BodyLogAvatar5ModID, false);
                    }
                    if (!Main.IsBarcodeInGame(data.BodyLogAvatar6))
                    {
                        Main.DownloadModIOMod(data.BodyLogAvatar6ModID, false);
                    }


                    Main.ChangeBodyLogAvatarSlot(1, data.BodyLogAvatar1, false);
                    Main.ChangeBodyLogAvatarSlot(2, data.BodyLogAvatar2, false);
                    Main.ChangeBodyLogAvatarSlot(3, data.BodyLogAvatar3, false);
                    Main.ChangeBodyLogAvatarSlot(4, data.BodyLogAvatar4, false);
                    Main.ChangeBodyLogAvatarSlot(5, data.BodyLogAvatar5, false);
                    Main.ChangeBodyLogAvatarSlot(6, data.BodyLogAvatar6, false);
                    Main.NotificationNowAlways(FusionProtectorInfo.ClientName, $"Applied {stringplayername} Bodylog To Yours!",NotificationType.SUCCESS,3.5f);
                });
            }

        }
    }
    public class SendNotificationData : INetSerializable
    {

        public byte smallId;
        public string messagedata;
        public string title;
        public float length;

        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref messagedata);
            serializer.SerializeValue(ref title);
            serializer.SerializeValue(ref length);
        }

        public static SendNotificationData Create(byte smallId, string messagedatax, string title, float length)
        {
            return new SendNotificationData()
            {
                smallId = smallId,
                messagedata = messagedatax,
                title = title,
                length = length
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class SendNotificationMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 5f;
        public static float _timeOfRequest = -1000f;
        protected override void OnHandleMessage(ReceivedMessage received)
        {

            if (!Main.playermessaging)
            {
                return;
            }

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;
            var data = received.ReadData<SendNotificationData>();
            var senderId = received.Sender.Value;


            if (data.smallId != received.Sender.Value)
                return;

    
                Notifier.Send(new Notification
                {
                    Title = data.title,
                    Message = new NotificationText(data.messagedata, Color.yellow),
                    SaveToMenu = true,
                    ShowPopup = true,
                    PopupLength = data.length,
                    Type = NotificationType.WARNING,
                    OnAccepted = () => { }
                });
            
        }
    }
    public class ProtectorPingData : INetSerializable
    {
        public string versionoffusionprotector;
        public byte smallId;
        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref versionoffusionprotector);
        }
        public static ProtectorPingData Create(byte smallId, string versionoffp)
        {
            return new ProtectorPingData()
            {
                smallId = smallId,
                versionoffusionprotector = versionoffp
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class ProtectorPingMessage : ModuleMessageHandler
    {
        public static IEnumerator KickOutdatedUser(PlayerID PToKick)
        {
            if (!NetworkPlayerManager.TryGetPlayer(PToKick, out var playernow))
                yield break;

            var playerId = playernow.PlayerID;

            Main.NotificationNow(
                FusionProtectorInfo.ClientName,
                $"{Main.CleanedNAME(playernow)}\nWill Be Kicked For Using OUTDATED Fusion Protector In 10 Seconds!.",
                NotificationType.INFORMATION,
                5.0f,
                true,
                true,
                () =>
                {
                    Main.CheckSteamID(playernow.JR_SteamID());
                });

            var data = SendNotificationData.Create(
            PlayerIDManager.LocalSmallID,
            $"You Will Be Kicked For Using OUTDATED Fusion Protector In 10 Seconds!.",
            FusionProtectorInfo.ClientName,
            5.0f
            );

            MessageRelay.RelayModule<SendNotificationMessage, SendNotificationData>(
                data,
                new MessageRoute(PToKick.SmallID, NetworkChannel.Reliable)
            );

            yield return new WaitForSecondsRealtime(10.0f);

            if (NetworkPlayerManager.TryGetPlayer(playerId, out var currentPlayer))
            {
                NetworkHelper.KickUser(currentPlayer.PlayerID);
            }
        }


        protected override void OnHandleMessage(ReceivedMessage received)
        {
           
            var data = received.ReadData<ProtectorPingData>();

            if (data.smallId != received.Sender.Value)
                return;

            if (!Version.TryParse(data.versionoffusionprotector, out var playerVersion))
                return;

            if (!NetworkPlayerManager.TryGetPlayer(data.smallId, out var player))
                return;

            string title = FusionProtectorInfo.ClientName;
            string message = $"{Main.CleanedNAME(player)}\nI'm Using v{data.versionoffusionprotector}!";
            NotificationType type = NotificationType.SUCCESS;
            float duration = 5.0f;

            if (playerVersion < Main.FPVersionCurrent)
            {
                title = "[OLD] " + title;
                type = NotificationType.WARNING;

                if (Main.autokickoldfusionprotectorusers && NetworkInfo.IsHost)
                {
                    MelonCoroutines.Start(KickOutdatedUser(player.PlayerID));
                }
            }
            else if (playerVersion > Main.FPVersionCurrent)
            {
                title = "[Newer Version] " + title;
                duration = 6.0f;
            }

            Main.NotificationNow(
                title,
                message,
                type,
                duration,
                true,
                true,
                () => Main.CheckSteamID(player.JR_SteamID())
            );
        }
    }
    public class OwnerServerSettingData : INetSerializable
    {
        public byte smallId;
        public string serversettings;

        public enum ValueType
        {
            Bool,
            Int,
            Float,
            String
        }

        public ValueType valueType;
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;


        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref serversettings);
            serializer.SerializeValue(ref valueType);
            serializer.SerializeValue(ref boolValue);
            serializer.SerializeValue(ref intValue);
            serializer.SerializeValue(ref floatValue);
            serializer.SerializeValue(ref stringValue);
        }

        public static OwnerServerSettingData Create(byte smallId, string serversetting, object valuenow)
        {
            var data = new OwnerServerSettingData
            {
                smallId = smallId,
                serversettings = serversetting
            };

            if (valuenow == null)
                throw new Exception("Value cannot be null.");

            var valueType = valuenow.GetType();
            if (valueType.IsEnum)
            {
                data.valueType = ValueType.Int; 
                data.intValue = Convert.ToInt32(valuenow);
                return data;
            }

            switch (valuenow)
            {
                case bool b:
                    data.valueType = ValueType.Bool;
                    data.boolValue = b;
                    break;

                case int i:
                    data.valueType = ValueType.Int;
                    data.intValue = i;
                    break;

                case float f:
                    data.valueType = ValueType.Float;
                    data.floatValue = f;
                    break;

                case string s:
                    data.valueType = ValueType.String;
                    data.stringValue = s;
                    break;

                default:
                    throw new Exception($"Unsupported value type: {valuenow.GetType()}");
            }

            return data;
        }

        public bool GetBool() => valueType == ValueType.Bool ? boolValue : false;
        public int GetInt() => valueType == ValueType.Int ? intValue : 0;
        public float GetFloat() => valueType == ValueType.Float ? floatValue : 0f;
        public string GetString() => valueType == ValueType.String ? stringValue : null;
    }
    [Net.SkipHandleWhileLoading]
    public class OwnerServerSettingMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 5f;
        public static float _timeOfRequest = -1000f;
        static void AlertHost(NetworkPlayer nowPlayer, string settingName, object value)
        {
            Main.NotificationNow(
                FusionProtectorInfo.ClientName,
                $"{Main.CleanedNAME(nowPlayer)}\nEdited Server Setting {settingName} To Value : {value}",
                NotificationType.ERROR
            );
        }

        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.OWNERSCANCHANGESERVER)
                return; 
            
            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;

            var data = received.ReadData<OwnerServerSettingData>();
 
            if (data.smallId != received.Sender.Value) return;
            if (!NetworkPlayerManager.TryGetPlayer(data.smallId, out var playerNow)) return;

            FusionPermissions.FetchPermissionLevel(playerNow.JR_SteamID(), out var selfLevel, out _);
            if (selfLevel != PermissionLevel.OWNER) return;

            if (string.IsNullOrEmpty(data.serversettings)) return;

            bool newBoolValue = data.GetBool();
            var newStringValue = (PermissionLevel)data.GetInt();

            switch (data.serversettings)
            {
                
                case "NameTags":
                 Main.EditFusionPreferences("Server Nametags Enabled", newBoolValue);
                 AlertHost(playerNow, "NameTags", newBoolValue);
                    break;
                case "VoiceChat":
                    Main.EditFusionPreferences("Server Voicechat Enabled", newBoolValue);
                    AlertHost(playerNow, "VoiceChat", newBoolValue);
                    break;
                case "Mortality":
                    Main.EditFusionPreferences("Server Mortality", newBoolValue);
                    AlertHost(playerNow, "Mortality", newBoolValue);
                    break;
                case "Friendly Fire":
                    Main.EditFusionPreferences("Friendly Fire", newBoolValue);
                    AlertHost(playerNow, "Friendly Fire", newBoolValue);
                    break;
                case "Knockout":
                    Main.EditFusionPreferences("Knockout", newBoolValue);
                    AlertHost(playerNow, "Knockout", newBoolValue);
                    break;
                case "Player Constraining":
                    Main.EditFusionPreferences("Server Player Constraints Enabled", newBoolValue);
                    AlertHost(playerNow, "Player Constraining", newBoolValue);
                    break;
                case "Dev Tools":
                    Main.EditFusionPreferences("Dev Tools Allowed", newStringValue);
                    AlertHost(playerNow, "Dev Tools", newStringValue);
                    break;
                case "Constrainer":
                    Main.EditFusionPreferences("Constrainer Allowed", newStringValue);
                    AlertHost(playerNow, "Constrainer", newStringValue);
                    break;
                case "Custom Avatars":
                    Main.EditFusionPreferences("Custom Avatars Allowed", newStringValue);
                    AlertHost(playerNow, "Custom Avatars", newStringValue);
                    break;
                case "Teleportation":
                    Main.EditFusionPreferences("Teleportation", newStringValue);
                    AlertHost(playerNow, "Teleportation", newStringValue);
                    break;
            }

            LobbyInfoManager.PushLobbyUpdate();
        }
    }
    public class SendGameModeOverData : INetSerializable
    {

        public byte smallId;
        public string gamemode;

        public void Serialize(INetSerializer serializer)
        {
            serializer.SerializeValue(ref smallId);
            serializer.SerializeValue(ref gamemode);
        }

        public static SendGameModeOverData Create(byte smallId, string gamemodex)
        {
            return new SendGameModeOverData()
            {
                smallId = smallId,
                gamemode = gamemodex
            };
        }
    }
    [Net.SkipHandleWhileLoading]
    public class SendGameModeOverMessage : ModuleMessageHandler
    {
        public const float _requestCooldown = 5f;
        public static float _timeOfRequest = -1000f;
        protected override void OnHandleMessage(ReceivedMessage received)
        {
            if (!Main.ownerscanchangegamemode)
            {
                return;
            }

            if (TimeReferences.TimeSinceStartup - _timeOfRequest <= _requestCooldown)
            {
                return;
            }

            _timeOfRequest = TimeReferences.TimeSinceStartup;

            var data = received.ReadData<SendGameModeOverData>();
           
            if (data.smallId != received.Sender.Value)
                return;   
            
            if (!NetworkPlayerManager.TryGetPlayer(data.smallId, out var playerNow)) return;

            FusionPermissions.FetchPermissionLevel(playerNow.JR_SteamID(), out var selfLevel, out _);
            if (selfLevel != PermissionLevel.OWNER) return;

            if (string.IsNullOrEmpty(data.gamemode)) return;


            if (data.gamemode == "sandbox")
            {
                Main.NotificationNow(FusionProtectorInfo.ClientName, $"Gamemode Was Changed By {Main.CleanedNAME(playerNow)} To [Sandbox]", NotificationType.WARNING, 5.0f);

                GamemodeManager.DeselectGamemode();
            }
            else
            {
                string actualgamemodenow = "";
                
                switch (data.gamemode)
                {
                    case "Lakatrazz.Deathmatch": actualgamemodenow = "Deathmatch"; break;
                    case "Lakatrazz.Juggernaut": actualgamemodenow = "Juggernaut"; break;
                    case "Lakatrazz.Entangled": actualgamemodenow = "Entangled"; break;
                    case "Lakatrazz.Smash Bones": actualgamemodenow = "Smash Bones"; break;
                    case "Lakatrazz.Hide And Seek": actualgamemodenow = "Hide And Seek"; break;
                    case "sandbox": actualgamemodenow = "Sandbox"; break;
                    case "Lakatrazz.Team Deathmatch": actualgamemodenow = "Team Deathmatch"; break;
                }

                if (GamemodeManager.TryGetGamemode(data.gamemode, out var gamemode))
                {
                    Main.NotificationNow(FusionProtectorInfo.ClientName, $"Gamemode Was Changed By {Main.CleanedNAME(playerNow)} To [{actualgamemodenow}]", NotificationType.WARNING, 5.0f);
                    GamemodeManager.SelectGamemode(gamemode);
                }
            }
        }
    }
}
