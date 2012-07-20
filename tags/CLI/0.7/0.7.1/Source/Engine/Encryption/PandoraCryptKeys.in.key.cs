﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PandoraMusicBox.Engine.Encryption {
    partial class PandoraCryptKeys {
        private static void initializeInKey() {
            In = new BlowfishKey();

            In.N = 16;

            In.P = new uint[] {
		        0x7965742C, 0x4A205F3D, 0x143F8F89, 0xC976E0B1, 0x37D227B0, 0x78968D06, 
                0x9F28933C, 0xD21A7537, 0x80EB812C, 0xE5A60D9B, 0xF2B6B13D, 0x67079BAF, 
                0x0C73A7C2, 0x95D331DD, 0x80379EC4, 0x16B753B1, 0xC23F34AE, 0x7A3C45D7
		    };

            In.S = new uint[,] {{ // 4 x 256
				0x58AF6ECE, 0x6B306780, 0x033EF993, 0x4299C20B, 0x47ADC709, 0xDB40EE14, 
                0x3772FA47, 0x473385D9, 0xBFC0AF75, 0xD439AE96, 0x6A2EF2EE, 0x4A25F261, 
                0x69345881, 0x65DD6DFC, 0x7A87B813, 0x626A4332, 0x675A3E91, 0x2C19B6DA, 
                0x62108522, 0x26CB31B9, 0x584DF87D, 0x5024976F, 0x48136869, 0x5C56CBA9, 
                0x5AD39E1B, 0x133F6EBA, 0xB1C66E67, 0x90880621, 0xA9886ABC, 0x5AAFB5FD, 
                0x2623955D, 0x737CC474, 0xD5248060, 0x67C4B493, 0xBAC12128, 0x095810AB, 
                0x613AB2F2, 0x30E1B44A, 0x8291449B, 0xAF474E70, 0x6CD5307B, 0xB13AD61D, 
                0x721871F8, 0xFD55DB7F, 0x7415A01C, 0x580B8CA6, 0x284FE1B9, 0xA4F0BD0D, 
                0x7BF1167D, 0x82662FC7, 0xC7524E17, 0x2F7C69A2, 0x089FA280, 0x90E18CD8, 
                0x70536F17, 0xF5E7ED0D, 0x13388A46, 0x9DB0CECE, 0xC6710FE3, 0x00E399AD, 
                0x22E77D76, 0x63CDE083, 0x757D804E, 0xF821AEAD, 0xF84B66E9, 0xE6BC3E7C, 
                0x5DFC3E57, 0x158C599D, 0x27DEDF6B, 0x777BF721, 0x05D82093, 0x8B2BC85F, 
                0x09918B2F, 0xF4C702E8, 0xDF00CD28, 0x491A4FAD, 0x64944EE2, 0x872ED2E7, 
                0xF3288DB7, 0x1F93D679, 0xAD42DD2D, 0xE8131A69, 0xD8BA3A70, 0x73F86D65, 
                0xB3C72776, 0x52CC70C8, 0xABA8C646, 0x4A323B09, 0x7D482403, 0x9E03399D, 
                0x2B717494, 0x6BED832B, 0xF8A661BA, 0xC07E4F5E, 0x589460BC, 0x1DA78D74, 
                0xD8ECD29F, 0xBA3ED619, 0xF2D647B0, 0xAF86F7A8, 0x4CA53870, 0xBFECF67F, 
                0xA778B6FE, 0x84D56E44, 0x1F4F61ED, 0x1F8329E1, 0xEDD3E331, 0x27F854E3, 
                0x2DA40439, 0xFBC0BB45, 0x91327B1F, 0xC819276C, 0x72AD0FAE, 0xDE13B223, 
                0xD2F381DC, 0x826BB46D, 0x295BC153, 0x9048AC23, 0x945605D9, 0x944D59CB, 
                0xBA1A643D, 0xA16F9E33, 0xED95325E, 0xB1E5E9CA, 0xC2233F09, 0x44585853, 
                0x6A4EEC8F, 0xF93C1555, 0xD6793587, 0xE934216B, 0x3A8332B3, 0x3A8466C9, 
                0xAC7386CC, 0x01668A9F, 0xA28FF66F, 0xDA303600, 0xD6E18E43, 0x3D592ADA, 
                0xDE2C3640, 0x8DF5BD6B, 0x1AB26FBB, 0xE59EC9E8, 0xAC9925B3, 0xC227130C, 
                0x467A9AF0, 0xA9579945, 0x0E1652A4, 0x433805AF, 0x4AE0F0FD, 0xD9218763, 
                0x54D623FF, 0x39BD38C8, 0xC639E971, 0xEFED7056, 0xCF46F0D3, 0x0A43FB36, 
                0xE73E362E, 0x092400F6, 0x242821E7, 0xC3953CDB, 0x8C02D71C, 0xD9D5B909, 
                0x64B442AF, 0x29D5FFBA, 0xB479B691, 0x5AA9A01C, 0x49CBD1C9, 0x41EAFBF8, 
                0x888144A6, 0x844C076D, 0x05581523, 0xC5E98FFD, 0x13056FE1, 0xA4056B01, 
                0x09F53013, 0x0AD00575, 0xACB8354D, 0x52ECE455, 0xFD8890D3, 0xAF651F23, 
                0xAD7374D2, 0x99CCEAB5, 0x2F0F603D, 0x5E7EA504, 0x608963E1, 0xC1BD2196, 
                0x200B27B3, 0xD9D1E761, 0xEFF36E5A, 0x547B24C8, 0x7C7F77BC, 0xA9E78393, 
                0x6B9F3172, 0xC6529DBD, 0xB6E0011D, 0x40CDA153, 0xE74DDD18, 0x01A98B3B, 
                0xD9B6F384, 0x57AAA89B, 0x98F36734, 0x98BAAA5A, 0x47F961DE, 0x12803DCB, 
                0x24D3E504, 0xB5FA31A1, 0xCDA87476, 0x9CC48FC9, 0xBDD02CA2, 0xF5963721, 
                0x722CC439, 0x519EF966, 0xD5699454, 0xF8AEED1C, 0xC5EC22B8, 0x52D7EB6A, 
                0xC179828C, 0xB383272E, 0x206888FC, 0xAF1A692E, 0x217BF251, 0x6C0D0A71, 
                0x0C84184B, 0x79DD1780, 0x3B3F72A8, 0x33478E4B, 0x06BF0967, 0x9023FA3F, 
                0x8303A262, 0x7AC0E4A6, 0xD439DEB1, 0x1DBEF98B, 0xFEF0BE31, 0x1B87F008, 
                0x7C2196FF, 0xF5447601, 0xB1508F3A, 0x512CFD07, 0x3137B2D4, 0x768CFFC8, 
                0x970C456D, 0xC06D34B4, 0xE257E53D, 0x8C75C72B, 0xC9DB8A31, 0xDE84BB8F, 
                0x5B332228, 0x8BF79C5A, 0x0B3EFE49, 0xF0C4BF7E, 0xB958ED83, 0x5B37EE2D, 
                0xDB04C07A, 0x72739791, 0x55C40314, 0x5129C81C
            },{
                0x700C96F3, 0xDE2D98F3, 0x503D5563, 0xA5A92702, 0x5F87B11C, 0xC5FDF6C2, 
                0x9D5EADF9, 0x82D21E82, 0xBFBE92EC, 0x27B25533, 0xF6C9ABA1, 0x787D218D, 
                0xFDBF4423, 0x439ED927, 0x3201F7B4, 0xB8DFE640, 0x88AD318E, 0x2076AB45, 
                0xC8654627, 0x658D0920, 0x09FE3274, 0xF00FD288, 0xF3E47731, 0x6028108C, 
                0x98F52E66, 0x10B6F6C6, 0xFE6E6CBD, 0x18855CA0, 0x41B04EF1, 0x3A075160, 
                0x5158DE83, 0xFBB9F0C9, 0x5E3FDC6C, 0xD72EFEF8, 0x04C4EF61, 0x99EDDA29, 
                0xC653FE1E, 0x6B85E447, 0xBE07D9F5, 0x16CE88D4, 0x6BF376DD, 0xA12CEFDE, 
                0x22FC5353, 0x2890980D, 0x8B99543C, 0xAB2C42BC, 0x510892C5, 0x416951DD, 
                0x219D7D99, 0x5C83A431, 0x7F6B1F4E, 0x3CDDDEBD, 0xB96B4C75, 0xB88ADF78, 
                0x48D54415, 0xD89AA204, 0x85FA0A84, 0xCCEBA68C, 0x6FF06438, 0x0F3BAE05, 
                0xD2D85107, 0x19B91D81, 0x2C68AED8, 0xBBE8F8D2, 0xA26C27A8, 0xBA1B02E0, 
                0x90F091FE, 0xA62A3797, 0x9FC43203, 0x59393925, 0x354AA050, 0xA709B895, 
                0x6B8AA793, 0x4A679A6C, 0x47EEA590, 0x21AA4B78, 0xC103CEF9, 0x7832F982, 
                0x0A19AF36, 0x71253891, 0xA0C16436, 0x968852BD, 0x6694B976, 0x0884FB93, 
                0x46EB1E9F, 0xFA945C75, 0xD3C928FB, 0xD1C8BF8C, 0xAF20AAA9, 0x9FA86CD2, 
                0xDCCDED57, 0x1BDD4247, 0x94F91D5C, 0x7D6D5058, 0x11F0DB4E, 0xF9A48F09, 
                0xFFA3DFB4, 0xB27B4DE0, 0xDEAB8E3F, 0x20AD0F77, 0x9C13FF7C, 0x16ACC3A5, 
                0x59FD4711, 0xE13FC78E, 0x286B7532, 0x3352F5BB, 0xA3305FEB, 0x643CFC7B, 
                0x689DE9F4, 0x4EA0B270, 0x532DC782, 0xA5C504C3, 0xBFC29608, 0x0F3FD845, 
                0xD62C9C37, 0x8F9D345A, 0x7BCA7EB6, 0xDA8E1FCC, 0x152B59CE, 0x625BB739, 
                0x49A5AA8F, 0x24417D34, 0xE9C9ED1B, 0x0E20A019, 0xE81DBC3A, 0xEA7FDD74, 
                0xBD0A0794, 0x85585D33, 0xA48530D2, 0x991CC6AB, 0xA5488F6C, 0x4F1A494D, 
                0xB45F297F, 0x0F357907, 0x56574FEC, 0x4D4519FF, 0x2B78FBDF, 0x28CA6528, 
                0x095D79B6, 0x48CB1657, 0x6B56EED0, 0xB0CCBE78, 0xE702AEC1, 0x350BDFB7, 
                0x59E0E969, 0xA4154BA8, 0xBA56355C, 0x545028BC, 0xEF129A26, 0xC594C313, 
                0xF74051A7, 0x90F33DE7, 0x7946623B, 0x06875CF1, 0xA47F30CD, 0x3FD1EEE0, 
                0x848065A2, 0x4788DB48, 0x7AFFF19F, 0x1A6F58AA, 0xA929B0BE, 0x4297C802, 
                0xA5C9DB5C, 0x972DF7F5, 0xFB449508, 0xFA5E027F, 0x903D0ACC, 0xD9481446, 
                0x485F43F3, 0xE99D44BB, 0xF830B7D5, 0x7A8D521C, 0x84B98AFB, 0xE88C86DF, 
                0xF59C4CD1, 0x9F66E618, 0x71F390EC, 0x59C364EF, 0x47E57D97, 0xDB769D9B, 
                0x8A5DF152, 0xF3F1AFC2, 0x23791AA5, 0x6032C1E6, 0xCDCD381B, 0x88298F9A, 
                0x0489B57B, 0x7206785B, 0x086F2C1B, 0x779C61E9, 0xF87EA443, 0x57C8DA35, 
                0xA417C341, 0x7883BFF4, 0x165BEEFA, 0xE630556F, 0xE136B428, 0x65F03AB7, 
                0xC218B820, 0xC4DF8526, 0x2A4F4982, 0x124811E9, 0xF799A377, 0xFD1D0033, 
                0x663FB7EF, 0x1CCAFABC, 0x44AF1166, 0x5A164940, 0x848956EA, 0x6E6552B2, 
                0xF6EF98DD, 0x3BCF388D, 0xB054A3BC, 0x64EF380A, 0xB0DF7014, 0xBC6E2BF1, 
                0xF40268C2, 0xD4552EC1, 0xFC31E5DA, 0x8737009F, 0x8A644503, 0x76743771, 
                0x2A594CC4, 0x9BE48DE7, 0x0E750C92, 0x7790C8F1, 0x8E2B2824, 0x671BEF2D, 
                0x2FDFFAAB, 0x0A75C150, 0x9DB37E38, 0x964EC3A0, 0xC2F0BEF2, 0x4DEA50CC, 
                0x0E224E06, 0x7B5FB816, 0x256BF43C, 0x2E254562, 0xE4D05BB6, 0xB192839C, 
                0x0DCBC8E7, 0x45565F05, 0xDD0F61AE, 0x2AF501FE, 0x740CC6EE, 0x20A23735, 
                0x6D4C1A5F, 0xDF48E0F3, 0x841B7D9A, 0xEC88226D, 0x454937E6, 0xBD38C2FC, 
                0x67A5FE1C, 0x310DADE4, 0xF0544BA3, 0xE5077FDC
            },{
                0x3A9B9093, 0xA4633D29, 0x864616EF, 0x6306B63D, 0x6D40A577, 0x8472BE3A, 
                0x3F82F43C, 0x11B3DC89, 0x58B59414, 0x9625F326, 0x9732ECAA, 0x03BF67AD, 
                0x69FA01D4, 0x48CDEBB5, 0x2D5B8BC4, 0x37D5592C, 0xC7D6E32A, 0x33AD6F03, 
                0xA07D2033, 0x1ECB01F0, 0xC457C910, 0xFD4768A8, 0x60DF8140, 0x3F6DE965, 
                0xF705D74E, 0x8A72E059, 0x2205A9EB, 0xCE273AB1, 0xFA920510, 0x176E53C5, 
                0xDF4C779B, 0x0DF906CA, 0xB99317FA, 0x3F2951DF, 0xE8EB0716, 0xF4378364, 
                0xB2E5013B, 0x01C87633, 0xE1369E82, 0xB73812CC, 0x859E8144, 0x4FEFF8F3, 
                0x2C3B97A3, 0x7E8A3B4F, 0x2AE02629, 0xE3B078CF, 0x69555F9A, 0x9795B141, 
                0x2CB274C3, 0x0E7F8477, 0x765B20CF, 0xB908FF7D, 0xBD5F6FF5, 0x33DCAE67, 
                0x5223DC88, 0x8C777C0F, 0x257535A3, 0xAF772C03, 0xDBEE922F, 0xB9903499, 
                0x51A5C816, 0x1F566F58, 0xC56C5D6A, 0x5DAE7E5C, 0xBF2F4E5D, 0xEC994673, 
                0xD10292F9, 0x4807EBF7, 0x8CB1B02A, 0xC245F1A0, 0x967F40C2, 0x9C18FCE3, 
                0xAD6F9F84, 0xFFDACF8A, 0x383BA5C7, 0xB1062148, 0x9F8B5794, 0xF04B8B41, 
                0xF7065C1E, 0x2DF21206, 0xD2C19E57, 0x8A1D1724, 0x098807AE, 0xF1FFAD8C, 
                0x28C3B7AB, 0x15F08CB9, 0x819A0342, 0x9CA785AD, 0xBCA12936, 0x57005E72, 
                0xD2952717, 0xAA2C7A40, 0xBAD5C47A, 0x5E249A52, 0xE4F67168, 0xA24B0FD9, 
                0x0C74B46D, 0x2C6F753E, 0x271E8A9D, 0xBECFC090, 0x1AE87E40, 0xB8D370FE, 
                0xE55229B5, 0x4D4F8DF2, 0x5500EAED, 0x7077304A, 0x0CC88F39, 0x0C1A57FF, 
                0x65A15916, 0xEB25A56F, 0xCE051524, 0x6F3B6B29, 0xC377CA76, 0xA4B92E18, 
                0x6A65CE33, 0x9705BE40, 0x0EE9622C, 0xC151BCFA, 0xA4B920BC, 0x0B062E55, 
                0x907DFE6C, 0x2454EF6A, 0x639B7D23, 0x886FCBF0, 0x4BE37F14, 0xE841CA25, 
                0x19290F76, 0x6AC74F0C, 0xD77807FC, 0x38662787, 0x31FBAB00, 0x6E86D381, 
                0x6542EF1E, 0xCF0FD34D, 0xD76365FA, 0xB38A9713, 0x03CA5ED3, 0xDA72B659, 
                0x98449C9D, 0x5A4BB852, 0xE8B96682, 0x254D34DF, 0x8FA2C1B4, 0x2E8BBC0C, 
                0x9F8C0AD0, 0x4737AF25, 0x8D8DCD3F, 0x37AE4FE6, 0x3CA183DD, 0x5C6800A6, 
                0x8AC9FAB7, 0xA6A8560A, 0xF38FF50D, 0x7EF176AF, 0xE1CDE486, 0x1EFE0B95, 
                0x8AA0B26D, 0x6BB7E125, 0xAE3082DE, 0xB8B8693C, 0x260BC5EF, 0xAA4CA762, 
                0x96EE37D5, 0x92ED36AB, 0x4E64CBFE, 0x15302E8E, 0xC7EC0569, 0x7A3E62E4, 
                0x846A7554, 0x3A527824, 0x413E1BB7, 0xF277C0C0, 0x4AED3640, 0x070E7CF1, 
                0x34BA52AC, 0xB0E769AC, 0x173FF792, 0x54AB5BF9, 0x2C7FE691, 0x44E1DCFF, 
                0xCCAD04A0, 0xC36DF12C, 0x426046A1, 0x1815E1A6, 0x1D08D080, 0x2122A6F7, 
                0xFE0ADBB3, 0x3EE44567, 0x97EACFD6, 0x6C39B52C, 0xF1666890, 0x0C064E3B, 
                0x4F0CE499, 0xA57051C2, 0xBAFAECF1, 0x4BD81CD4, 0x323926D2, 0x0F486B0F, 
                0x0BF3059A, 0xB64882B4, 0x26356A23, 0xC409DCC1, 0xD0FAB32D, 0x9A6A9A3F, 
                0xCFE564F1, 0x662A0BAB, 0x3531647F, 0xD6D32083, 0x1470E956, 0x170F18DA, 
                0x7777D4AF, 0xD3C7D311, 0x31ED300F, 0xB0438514, 0x69596A4D, 0xC204CF7A, 
                0x9F359719, 0x1F7B1E3F, 0x6FCB0D4B, 0x006FF6A0, 0xC45DD3F3, 0x2004AC7A, 
                0xA659FC7D, 0x6BB525F7, 0x79C2468C, 0x69B66BC8, 0xACD88068, 0xBC177474, 
                0x9BB8CDB8, 0xC1847712, 0x198AB988, 0xFB914A8E, 0x58D8915E, 0x9A7546E5, 
                0x96F72399, 0x0ED06FDB, 0xA3DFA9A1, 0x7AFE55C8, 0xEFBE9837, 0x28FC70E9, 
                0x58D7F102, 0x96EFB6CE, 0xDCBE8B2C, 0x4E0B3A4D, 0xE6BDE8EC, 0xC85297E4, 
                0xC6A21317, 0x9AB106F5, 0xCFD005D5, 0x8DF74A34, 0x9DDDA71E, 0x455B9DA6, 
                0xD5D097F7, 0xCEEFC20D, 0x5ED612B3, 0xE3D05A19
            },{
                0x39F5F98F, 0x5F4AEDF0, 0xA78A4E15, 0xEFE018C6, 0x794C93CE, 0x619114C5, 
                0x9BF937BD, 0x11B0E9E8, 0x03EE0EBE, 0x3463A92A, 0xAD780118, 0xFEC71882, 
                0xDD2A4FEE, 0x16EBED33, 0x32A07F20, 0x07C860AD, 0xAEECF1CC, 0x59155142, 
                0xF9355FA0, 0x0C888F13, 0x9269F453, 0xBB030F9C, 0x4A7E7D1F, 0x7BDE69C6, 
                0xD6251060, 0x89EDD1D7, 0xBFC1FCE3, 0x800CD339, 0xBBDB406E, 0x29F830FC, 
                0xF0185BC8, 0xCB3E5DFC, 0x34BB9DE7, 0xB9BE2E7F, 0xDC0A256C, 0xA3CF476D, 
                0xE8146F0E, 0xA05F759C, 0xB207A38F, 0x5CEA7F07, 0x11966D9A, 0xB86AE0CA, 
                0xA7F7507B, 0x86456CAF, 0xDA2FC94A, 0xF40EE3D7, 0xB3B16D2A, 0x65985D5B, 
                0x1D568E9D, 0xBA2CA598, 0x6E2B6FB5, 0x51C61179, 0xE4541A9F, 0x491B2D44, 
                0x5A6684E3, 0xFA3A88F0, 0x4DF38D58, 0x3EB854B1, 0x640E2FC0, 0x131AC3A0, 
                0x559A919A, 0x3FC31514, 0xD688840C, 0x1C34479D, 0xA94C5267, 0xC9DBA308, 
                0xD3C74191, 0x8A7B1567, 0xCA88F7E1, 0x7EB2621A, 0x891B7145, 0xBB795C83, 
                0xBEA8A0DC, 0xA14CDAFE, 0xA4EF8A76, 0x11A4B6C4, 0xFDD49085, 0xD75B50C6, 
                0x7D250736, 0x36C9E67F, 0xF851EAA6, 0xDBDEBCD8, 0xADEB555C, 0xA1D73460, 
                0x804B6E19, 0x7143D204, 0x08825C66, 0x2303D8B6, 0x1C87B9DD, 0x221CDAFA, 
                0x3C8B6ECB, 0x866E4FDE, 0x7E6423F7, 0x176A26B7, 0x2E6A6D38, 0x1C91D2B1, 
                0x0A00CFCF, 0xF3AB1646, 0x4C7219EC, 0x461ECA91, 0x984DD5C4, 0xCAD2E054, 
                0x0154D6D2, 0x4AB7BCFB, 0x339E2BA5, 0x660F4C0B, 0xFB5527F2, 0xFDAD33B1, 
                0x654FAB58, 0xD03FC602, 0xE80A4CC3, 0x201ABCBE, 0x87AAAE96, 0x2B63614B, 
                0x8A99EC48, 0x10478493, 0xBA8DCA6C, 0xA0A16AB4, 0x35713CF7, 0x666CB206, 
                0x4C3CC644, 0x448530DF, 0x1C2633EC, 0x53AA9DFB, 0xA302DCE8, 0x2591E95E, 
                0x907278B0, 0x3DB7C94D, 0xD24995F5, 0x0D2D47C8, 0x62F6C46C, 0x4A7898ED, 
                0xF6A0B8B1, 0xC9E996BF, 0x709D7875, 0x114F9629, 0xF6A6AC6D, 0x49E81DE9, 
                0x01F352DA, 0xF7CF515F, 0x17687E75, 0xDE732D7F, 0xB0BC9739, 0x753FBF17, 
                0x256DD9C2, 0xB3825BB9, 0xFF1C5CF4, 0x1EB65A04, 0x15F13888, 0xB33C5B65, 
                0x39AC79F1, 0xFC2D0825, 0xC76CDC60, 0xC713543C, 0x7C03244C, 0x59D55BDB, 
                0x6C4F9986, 0xB179D387, 0x0D6B7585, 0x82650FD8, 0x0C402008, 0xB2B992DB, 
                0x1A98611B, 0x65BEC302, 0x3140C3BA, 0x6A0AE834, 0x040DE77E, 0xB5620AC0, 
                0x109F3480, 0xA8B6A324, 0x194ECE42, 0x5A1FFEFB, 0x4EE8E582, 0xA2C942BF, 
                0x4959B308, 0xBFC3D444, 0x7DFAD51F, 0xAFD87111, 0x1696895B, 0xE2C9C82D, 
                0x1FEBA9F8, 0xA10FECCD, 0xFFB77472, 0x06CE8942, 0x24761E62, 0x64190FC3, 
                0xF457DD2A, 0xA52CBA3F, 0xB3B3A04B, 0x93D21005, 0x4E560A41, 0xDE9D69FC, 
                0x9BA5755C, 0x24982126, 0x50308268, 0x4C371EB3, 0x11A9B36E, 0x5D589990, 
                0x153DF664, 0x9FA19C92, 0xE76AA4C4, 0xA5E7176B, 0x85701ED8, 0x9E80DB90, 
                0x1BC954E2, 0x52F1A00F, 0xB86B2D16, 0x367A7FCD, 0xF3EC57C7, 0x5198F53D, 
                0x28B0881A, 0xEDD06DF9, 0xFCC06975, 0x2A47FE4A, 0xA2ED56CD, 0xFC36DBDC, 
                0xD2D6F278, 0xD1FDAD09, 0x8E274B1E, 0xD24F7DE2, 0x6304D5C8, 0xDD4B9B0C, 
                0x77830F46, 0x2731EA83, 0xC6269ADE, 0x833B38E7, 0xAAF9B7F0, 0x1DF7E21A, 
                0xEF33AF8A, 0x22DA6BC6, 0x4BBDCD98, 0xE31870A1, 0x55126353, 0x6D455688, 
                0x31CF5AA4, 0x94A4C5F9, 0xBF813C9D, 0xEB4D03A2, 0x930F74BF, 0xBFA60117, 
                0x84E6954A, 0x6B4C992A, 0xA1B1BE37, 0xD13F76EC, 0x31D32BF9, 0xD6F43033, 
                0x173E7BD2, 0xA0417167, 0x53540194, 0x1384FDAF, 0xBFBA75B6, 0xDDA06AEF, 
                0x5040678E, 0x73FC27E1, 0xCDF96A1A, 0x88E3E947
           }};
        }
    }
}
