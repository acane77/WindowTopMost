using System.Globalization;
using MelinaWindowTopmost.Models;

namespace MelinaWindowTopmost.Services;

public sealed class LocalizationService
{
    private static readonly Dictionary<string, Dictionary<string, string>> Resources = new()
    {
        ["zh-Hans"] = new Dictionary<string, string>
        {
            ["AppName"] = "Melina Window Topmost",
            ["SearchPlaceholder"] = "搜索窗口或进程",
            ["SearchWindowTitle"] = "搜索窗口名称",
            ["Refresh"] = "刷新",
            ["AutoRefresh"] = "自动刷新",
            ["Interval"] = "刷新间隔",
            ["Seconds"] = "秒",
            ["Settings"] = "设置",
            ["About"] = "关于",
            ["Windows"] = "窗口",
            ["Details"] = "详情",
            ["NoSelection"] = "请从列表中选择一个窗口。",
            ["Topmost"] = "置顶",
            ["Normal"] = "普通",
            ["SetTopmost"] = "设为置顶",
            ["CancelTopmost"] = "取消置顶",
            ["SwitchTo"] = "切换到窗口",
            ["OpenLocation"] = "打开位置",
            ["Information"] = "信息",
            ["Opacity"] = "透明度",
            ["Handle"] = "句柄",
            ["Title"] = "标题",
            ["Process"] = "进程",
            ["Path"] = "路径",
            ["Pid"] = "PID",
            ["Theme"] = "主题",
            ["Language"] = "语言",
            ["Backdrop"] = "背景材质",
            ["BackdropNone"] = "无",
            ["Acrylic"] = "亚克力",
            ["ClearIconCache"] = "清理图标缓存",
            ["Close"] = "关闭",
            ["Save"] = "保存",
            ["System"] = "跟随系统",
            ["Light"] = "浅色",
            ["Dark"] = "深色",
            ["License"] = "许可证：项目外壳使用 MIT 兼容授权。",
            ["Libraries"] = "第三方库：WPF UI、CommunityToolkit.Mvvm。",
            ["RefreshFailed"] = "刷新窗口列表失败。",
            ["ActionFailed"] = "操作失败。如果目标程序以管理员权限运行，请也用管理员权限运行本程序。",
            ["NoProcessPath"] = "无法获取进程路径。",
            ["CacheCleared"] = "图标缓存已清理。",
            ["MenuSwitchTo"] = "切换至该窗口",
            ["MenuTopMost"] = "窗口置顶",
            ["MenuCancelTopmost"] = "取消置顶",
            ["MenuSetOpacity"] = "设置窗口透明度",
            ["MenuShowInfo"] = "显示信息",
            ["MenuOpenLocation"] = "打开进程位置",
            ["MenuOpenUwpInstallPath"] = "\u6253\u5f00UWP\u5b89\u88c5\u8def\u5f84",
            ["MenuOpacityHidden"] = "0%（隐藏窗口）",
            ["MenuCustom"] = "自定义..."
        },
        ["zh-Hant"] = new Dictionary<string, string>
        {
            ["AppName"] = "Melina Window Topmost",
            ["SearchPlaceholder"] = "搜尋視窗或行程",
            ["SearchWindowTitle"] = "搜尋視窗名稱",
            ["Refresh"] = "重新整理",
            ["AutoRefresh"] = "自動重新整理",
            ["Interval"] = "重新整理間隔",
            ["Seconds"] = "秒",
            ["Settings"] = "設定",
            ["About"] = "關於",
            ["Windows"] = "視窗",
            ["Details"] = "詳情",
            ["NoSelection"] = "請從清單中選擇一個視窗。",
            ["Topmost"] = "置頂",
            ["Normal"] = "一般",
            ["SetTopmost"] = "設為置頂",
            ["CancelTopmost"] = "取消置頂",
            ["SwitchTo"] = "切換到視窗",
            ["OpenLocation"] = "開啟位置",
            ["Information"] = "資訊",
            ["Opacity"] = "透明度",
            ["Handle"] = "句柄",
            ["Title"] = "標題",
            ["Process"] = "行程",
            ["Path"] = "路徑",
            ["Pid"] = "PID",
            ["Theme"] = "主題",
            ["Language"] = "語言",
            ["Backdrop"] = "背景材質",
            ["BackdropNone"] = "無",
            ["Acrylic"] = "壓克力",
            ["ClearIconCache"] = "清理圖示快取",
            ["Close"] = "關閉",
            ["Save"] = "儲存",
            ["System"] = "跟隨系統",
            ["Light"] = "淺色",
            ["Dark"] = "深色",
            ["License"] = "授權條款：專案外殼使用 MIT 相容授權。",
            ["Libraries"] = "第三方程式庫：WPF UI、CommunityToolkit.Mvvm。",
            ["RefreshFailed"] = "重新整理視窗清單失敗。",
            ["ActionFailed"] = "操作失敗。如果目標程式以系統管理員權限執行，請也用系統管理員權限執行本程式。",
            ["NoProcessPath"] = "無法取得行程路徑。",
            ["CacheCleared"] = "圖示快取已清理。",
            ["MenuSwitchTo"] = "切換至該視窗",
            ["MenuTopMost"] = "視窗置頂",
            ["MenuCancelTopmost"] = "取消置頂",
            ["MenuSetOpacity"] = "設定視窗透明度",
            ["MenuShowInfo"] = "顯示資訊",
            ["MenuOpenLocation"] = "開啟行程位置",
            ["MenuOpenUwpInstallPath"] = "\u958b\u555fUWP\u5b89\u88dd\u8def\u5f91",
            ["MenuOpacityHidden"] = "0%（隱藏視窗）",
            ["MenuCustom"] = "自訂..."
        },
        ["ja"] = new Dictionary<string, string>
        {
            ["AppName"] = "Melina Window Topmost",
            ["SearchPlaceholder"] = "ウィンドウまたはプロセスを検索",
            ["SearchWindowTitle"] = "ウィンドウ名を検索",
            ["Refresh"] = "更新",
            ["AutoRefresh"] = "自動更新",
            ["Interval"] = "更新間隔",
            ["Seconds"] = "秒",
            ["Settings"] = "設定",
            ["About"] = "情報",
            ["Windows"] = "ウィンドウ",
            ["Details"] = "詳細",
            ["NoSelection"] = "一覧からウィンドウを選択してください。",
            ["Topmost"] = "最前面",
            ["Normal"] = "通常",
            ["SetTopmost"] = "最前面に設定",
            ["CancelTopmost"] = "最前面を解除",
            ["SwitchTo"] = "ウィンドウに切り替え",
            ["OpenLocation"] = "場所を開く",
            ["Information"] = "情報",
            ["Opacity"] = "透明度",
            ["Handle"] = "ハンドル",
            ["Title"] = "タイトル",
            ["Process"] = "プロセス",
            ["Path"] = "パス",
            ["Pid"] = "PID",
            ["Theme"] = "テーマ",
            ["Language"] = "言語",
            ["Backdrop"] = "背景素材",
            ["BackdropNone"] = "なし",
            ["Acrylic"] = "アクリル",
            ["ClearIconCache"] = "アイコンキャッシュをクリア",
            ["Close"] = "閉じる",
            ["Save"] = "保存",
            ["System"] = "システムに従う",
            ["Light"] = "ライト",
            ["Dark"] = "ダーク",
            ["License"] = "ライセンス: プロジェクトシェルは MIT 互換ライセンスを使用します。",
            ["Libraries"] = "サードパーティライブラリ: WPF UI、CommunityToolkit.Mvvm。",
            ["RefreshFailed"] = "ウィンドウ一覧の更新に失敗しました。",
            ["ActionFailed"] = "操作に失敗しました。対象プログラムが管理者権限で実行されている場合は、本プログラムも管理者権限で実行してください。",
            ["NoProcessPath"] = "プロセスパスを取得できません。",
            ["CacheCleared"] = "アイコンキャッシュをクリアしました。",
            ["MenuSwitchTo"] = "このウィンドウに切り替え",
            ["MenuTopMost"] = "ウィンドウを最前面に固定",
            ["MenuCancelTopmost"] = "最前面を解除",
            ["MenuSetOpacity"] = "ウィンドウの透明度を設定",
            ["MenuShowInfo"] = "情報を表示",
            ["MenuOpenLocation"] = "プロセスの場所を開く",
            ["MenuOpenUwpInstallPath"] = "UWP\u306e\u30a4\u30f3\u30b9\u30c8\u30fc\u30eb\u30d1\u30b9\u3092\u958b\u304f",
            ["MenuOpacityHidden"] = "0%（ウィンドウを隠す）",
            ["MenuCustom"] = "カスタム..."
        },
        ["en"] = new Dictionary<string, string>
        {
            ["AppName"] = "Melina Window Topmost",
            ["SearchPlaceholder"] = "Search windows or processes",
            ["SearchWindowTitle"] = "Search Window Name",
            ["Refresh"] = "Refresh",
            ["AutoRefresh"] = "Auto Refresh",
            ["Interval"] = "Refresh Interval",
            ["Seconds"] = "Seconds",
            ["Settings"] = "Settings",
            ["About"] = "About",
            ["Windows"] = "Windows",
            ["Details"] = "Details",
            ["NoSelection"] = "Please select a window from the list.",
            ["Topmost"] = "Topmost",
            ["Normal"] = "Normal",
            ["SetTopmost"] = "Set as Topmost",
            ["CancelTopmost"] = "Cancel Topmost",
            ["SwitchTo"] = "Switch to Window",
            ["OpenLocation"] = "Open Location",
            ["Information"] = "Information",
            ["Opacity"] = "Opacity",
            ["Handle"] = "Handle",
            ["Title"] = "Title",
            ["Process"] = "Process",
            ["Path"] = "Path",
            ["Pid"] = "PID",
            ["Theme"] = "Theme",
            ["Language"] = "Language",
            ["Backdrop"] = "Backdrop",
            ["BackdropNone"] = "None",
            ["Acrylic"] = "Acrylic",
            ["ClearIconCache"] = "Clear Icon Cache",
            ["Close"] = "Close",
            ["Save"] = "Save",
            ["System"] = "Follow System",
            ["Light"] = "Light",
            ["Dark"] = "Dark",
            ["License"] = "License: The project shell uses an MIT-compatible license.",
            ["Libraries"] = "Third-party libraries: WPF UI, CommunityToolkit.Mvvm.",
            ["RefreshFailed"] = "Failed to refresh the window list.",
            ["ActionFailed"] = "Operation failed. If the target program is running as administrator, please also run this program as administrator.",
            ["NoProcessPath"] = "Unable to get process path.",
            ["CacheCleared"] = "Icon cache has been cleared.",
            ["MenuSwitchTo"] = "Switch to this window",
            ["MenuTopMost"] = "Window Topmost",
            ["MenuCancelTopmost"] = "Cancel Topmost",
            ["MenuSetOpacity"] = "Set Window Opacity",
            ["MenuShowInfo"] = "Show Information",
            ["MenuOpenLocation"] = "Open Process Location",
            ["MenuOpenUwpInstallPath"] = "Open UWP Install Path",
            ["MenuOpacityHidden"] = "0% (Hide Window)",
            ["MenuCustom"] = "Custom..."
        }
    };

    private string _cultureName = "en";

    public AppLanguage Language { get; private set; } = AppLanguage.System;

    public void Apply(AppLanguage language)
    {
        Language = language;
        _cultureName = language switch
        {
            AppLanguage.ZhHans => "zh-Hans",
            AppLanguage.ZhHant => "zh-Hant",
            AppLanguage.Ja => "ja",
            AppLanguage.En => "en",
            _ => ResolveSystemCulture()
        };

        CultureInfo culture = CultureInfo.GetCultureInfo(_cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public string this[string key] => Translate(key);

    public string Translate(string key)
    {
        if (Resources.TryGetValue(_cultureName, out Dictionary<string, string>? resource) &&
            resource.TryGetValue(key, out string? value))
        {
            return value;
        }

        return Resources["en"].TryGetValue(key, out string? fallback) ? fallback : key;
    }

    private static string ResolveSystemCulture()
    {
        string name = CultureInfo.CurrentUICulture.Name;
        if (name.StartsWith("zh-Hant", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("zh-TW", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("zh-HK", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-Hant";
        }

        if (name.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-Hans";
        }

        if (name.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
        {
            return "ja";
        }

        return "en";
    }
}
