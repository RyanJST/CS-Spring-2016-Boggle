﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Boggle
{
    [DataContract]
    public class GameStatus
    {
        [DataMember]
        public string GameState { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Board { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string TimeLimit { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string TimeLeft { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public UserInfo Player1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public UserInfo Player2 { get; set; }

        public TimeSpan StartGameTime;
    }


    [DataContract]
    public class UserInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public string Nickname { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string Score { get; set; }

        public string UserToken { get; set; }
        
        public List<WordScore> WordsPlayed { get; set; }

    }

    [DataContract]

    public class UserTokenClass
    {
        [DataMember]
        public string UserToken { get; set; }
    }

    public class GameJoin
    {
        public string UserToken { get; set; }
        public string TimeLimit { get; set; }
        
    }
    public class UserGame
    {
        public string Word { get; set; }
        public string UserToken { get; set; }
    }

    public class WordScore
    {
        public string Word { get; set; }

        public int Score { get; set; }
    }

}