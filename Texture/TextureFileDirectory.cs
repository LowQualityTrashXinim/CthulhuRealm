namespace CthulhuRealm.Texture
{
    public static class ModTexture
    {
        public const string CommonTextureStringPattern = "JamUtilities/Texture/";
        public const string MissingTexture_Folder = "MissingTexture/";
        public const string PinIcon = CommonTextureStringPattern + "UI/PinIcon";

        public const string WHITEDOT = "JamUtilities/Texture/WhiteDot";
        public const string MISSINGTEXTUREPOTION = "JamUtilities/Texture/MissingTexturePotion";
        public const string EMPTYBUFF = "JamUtilities/Texture/EmptyBuff";
        public const string WHITEBALL = "JamUtilities/Texture/WhiteBall";
        public const string DIAMONDSWOTAFFORB = "JamUtilities/Texture/DiamondSwotaffOrb";
        public const string ACCESSORIESSLOT = "Terraria/Images/Inventory_Back7";
        public const string MENU = "JamUtilities/Texture/UI/menu";
        public const string SMALLWHITEBALL = "JamUtilities/Texture/smallwhiteball";
        public const string Lock = "JamUtilities/Texture/UI/lock";
        public const string Arrow_Left = CommonTextureStringPattern + "UI/LeftArrow";
        public const string Arrow_Right = CommonTextureStringPattern + "UI/RightArrow";
        public const string OuterInnerGlow = CommonTextureStringPattern + "OuterInnerGlow";
        /// <summary>
        /// Width : 16 | Height : 16
        /// </summary>
        public const string Boxes = CommonTextureStringPattern + "UI/Boxes";
        public const string QuestionMark_Help = CommonTextureStringPattern + "UI/Help";
        public const string Page_StateSelected = CommonTextureStringPattern + "UI/page_selected";
        public const string Page_StateUnselected = CommonTextureStringPattern + "UI/page_unselected";
        public const string DrawBrush = CommonTextureStringPattern + "UI/Brush";
        public const string FillBucket = CommonTextureStringPattern + "UI/FillBucket";
        public static string Get_MissingTexture(string text) => CommonTextureStringPattern + MissingTexture_Folder + $"{text}MissingTexture";
        public const string MissingTexture_Default = CommonTextureStringPattern + MissingTexture_Folder + "MissingTextureDefault";
    }
}
