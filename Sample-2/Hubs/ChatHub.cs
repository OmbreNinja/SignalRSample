using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Threading.Tasks;
using Sample_2.Models;
using Microsoft.AspNet.SignalR.Hubs;

namespace Sample_2.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// 每个聊天用户链接完时触发
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            var userInfo = Manager.UserInfos.SingleOrDefault(x => x.Uid == Context.ConnectionId);
            if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.Uid))
            {
                Manager.UserInfos.Add(new Models.UserInfo { Uid = Context.ConnectionId });
            }
            return base.OnConnected();
        }

        /// <summary>
        /// 每个聊天用户退出时触发
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
            var userInfos = Manager.UserInfos.Where(x => x.Uid == Context.ConnectionId);
            foreach (var item in userInfos)
            {
                Manager.UserInfos.Remove(item);
            }
            GetUserInfos();
            return base.OnReconnected();
        }

        /// <summary>
        /// 刷新所有的聊天用户的用户列表
        /// </summary>
        public void GetUserInfos()
        {
            Clients.All.refreshUserInfoList(Newtonsoft.Json.JsonConvert.SerializeObject(Manager.UserInfos));
        }

        /// <summary>
        /// 设置自己的昵称
        /// </summary>
        /// <param name="nickName"></param>
        public void SetUserInfoToNickName(string nickName)
        {
            var userInfo = Manager.UserInfos.SingleOrDefault(x => x.Uid == Context.ConnectionId);
            if (userInfo != null && !string.IsNullOrWhiteSpace(userInfo.Uid))
            {
                userInfo.NickName = nickName;
            }
            GetUserInfos();
        }

        /// <summary>
        /// 发送消息给所有人
        /// </summary>
        /// <param name="strMessage"></param>
        public void Send(string strMessage)
        {
            Clients.All.addNewMessage(Manager.UserInfos.SingleOrDefault(x => x.Uid == Context.ConnectionId).NickName, strMessage);
        }

        /// <summary>
        /// 发送消息给指定个人
        /// </summary>
        /// <param name="Uid">目标用户Uid</param>
        /// <param name="strMessage">消息</param>
        public void SendByUid(string Uid, string strMessage)
        {
            Clients.Client(Uid).sendNewMessage(Newtonsoft.Json.JsonConvert.SerializeObject(Manager.UserInfos.SingleOrDefault(x => x.Uid == Context.ConnectionId)), strMessage);
        }


    }
}