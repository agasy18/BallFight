﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallNetModel
{
    public class BallEngine
    {
        private List<BallUser> conectedUsers = new List<BallUser>();
        public List<BallUser> ConectedUsers
        {
            get { return conectedUsers; }
        }
        private Dictionary<String, BallModel> ballModels = new Dictionary<String, BallModel>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public BallUser AddNewBallUser(BallUser user)
        {           
            var exists =
                from BallUser e in this.ConectedUsers
                where e.AppAddress == user.AppAddress
                select e;

            if (exists.Count() == 0)
            {
                var partner =
                    from BallUser e in this.ConectedUsers
                    where e.Key == user.Key
                    select e;

                if (partner.Count() > 2)        //this is reserved key, please use another key
                    return null;
                else if (partner.Count() == 1)  //partner has been found
                {
                    user.IsFirstUser = false;
                    ballModels.Add(user.Key, new BallModel());
                }
                else                            //first user with that key
                {
                    user.IsFirstUser = true;
                }

                this.ConectedUsers.Add(user);

                Console.WriteLine("New user connected: " + user);
                return user;
            }
            else
                return null;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newMessage"></param>
        public void AddNewMessage(BallMessage newMessage)
        {
            try
            {
                var ballModel = ballModels[newMessage.User.Key];
                
                if (newMessage.User.IsFirstUser)
                {
                    ballModel.Player1Margine = newMessage.Margine;
                }
                else
                {
                    var t = ballModel.Player2Margine;
                    t.Left = ballModel.BoardSize.Width - newMessage.Margine.Left - ballModel.PlayerSize.Width;
                    ballModel.Player2Margine = t;
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
       
        public List<BallMessage> GetNewMessages(BallUser user)
        {
            try
            {
                var ballModel = ballModels[user.Key];
                switch (ballModel.GameOverPlayer)
                {
                    case PlayerEnum.Player1:
                        massageP1 = "You lose";
                        massageP2 = "You won";
                        break;
                    case PlayerEnum.Player2:
                        massageP2 = "You lose";
                        massageP1 = "You won";
                        break;
                    default:
                        break;
                }
                ballModel.GameOverPlayer = PlayerEnum.None;
                var message = new BallMessage()
                {
                    User = user,
                    Message = "",
                    Date = DateTime.Now,
                    GameSpeed = ballModel.GameSpeed
                };

                try
                {
                    message.User2Name = conectedUsers.First(user2 => user2.Key == user.Key && user.AppAddress != user2.AppAddress).UserName;
                }
                catch{}

                if (user.IsFirstUser)
                {
                    message.BallMargine = ballModel.BallMagine;
                    message.Margine = ballModel.Player2Margine;
                }
                else
                {
                    var w = ballModel.BoardSize.Width - ballModel.BallPosition.X - ballModel.BallSize.Width;
                    var h = ballModel.BoardSize.Height - ballModel.BallPosition.Y - ballModel.BallSize.Height;
                    message.BallMargine = new System.Windows.Thickness(w, h, 0, 0);

                    w = ballModel.BoardSize.Width - ballModel.Player1Margine.Left - ballModel.PlayerSize.Width;
                    message.Margine = new System.Windows.Thickness(w, 0, 0, 0);
                }

                if (message.User.IsFirstUser)
                {
                    message.Message = massageP1;
                    massageP1 = "";
                }
                else
                {
                    message.Message = massageP2;
                    massageP2 = "";
                }

                return new List<BallMessage>() { message };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void RemoveUser(BallUser user)
        {
            try
            {
                this.ConectedUsers.RemoveAll(u => u.AppAddress == user.AppAddress);
                this.ballModels.Remove(user.Key);
            }
            catch
            { }
        }

        public string massageP1 { get; set; }

        public string massageP2 { get; set; }
    }
}
