namespace WaterCloud.DingTalk
{
    public enum MsgTypeEnums
    {
        text,
        file,
        link,
        markdown,
        oa,
        action_card
    }

    /// <summary>
    /// 消息接收类型
    /// </summary>
    public enum MsgUserType
    {
        UserList,
        DepartList,
        AllUser
    }
}
