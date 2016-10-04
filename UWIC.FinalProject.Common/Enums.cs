namespace UWIC.FinalProject.Common
{
    public enum FirstLevelCategory
    {
        Default,
        FunctionalCommand,
        MouseCommand,
        KeyboardCommand
    }

    public enum SecondLevelCategory
    {
        Default,
        BrowserCommand,
        InterfaceCommand,
        WebPageCommand,
    }

    public enum CommandType
    {
        back,
        start_dictation_mode,
        start_general_spelling,
        start_website_spelling,
        start_password_spelling,
        forth,
        go,
        refresh,
        stop,
        no,
        ok,
        chng_back_colour,
        chng_fore_colour,
        close_tab,
        go_to_tab,
        open_new_tab,
        yes,
        scroll_down,
        scroll_left,
        scroll_right,
        scroll_up,
        alter,
        backspace,
        capslock,
        control,
        down_arrow,
        enter,
        f5,
        left_arrow,
        right_arrow,
        space,
        tab,
        up_arrow,
        move,
        click,
        double_click,
        right_click,
        show_grid,
        hide_grid
    }

    public enum FunctionalCommandType
    {
        Go,
        Forward,
        Backward,
        Refresh,
        Stop,
        StartVoice,
        StopVoice
    }

    public enum Mode
    {
        CommandMode,
        DictationMode,
        WebsiteSpellMode,
        GeneralSpellMode,
        PasswordSpellMode
    }

    public enum CaseState
    {
        UpperCase,
        LowerCase,
        Default
    }

    public enum MessageBoxIcon
    {
        Error,
        Information
    }
}
