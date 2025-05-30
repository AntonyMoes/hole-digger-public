﻿/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;
using System.Text;
using Facebook.MiniJSON;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FBWindowsTournamentsManager : MonoBehaviour {

    public FBWindowsLogsManager Logger;
    public InputField Title;
    public InputField Image;
    public Dropdown SortOrder;
    public Dropdown ScoreFormat;
    public InputField Data;
    public InputField InitialScore;
    public InputField Score;
    public InputField ShareData;


    private Dictionary<string, string> ConvertDataToDict(string UTF8String)
    {
        Encoding unicode = Encoding.Unicode;
        var unicodeData = Encoding.Convert(Encoding.UTF8, unicode, Encoding.UTF8.GetBytes(UTF8String));

        char[] unicodeDataChars = new char[unicode.GetCharCount(unicodeData, 0, unicodeData.Length)];
        Encoding.Unicode.GetChars(unicodeData, 0, unicodeData.Length, unicodeDataChars, 0);

        var data = Json.Deserialize(new string(unicodeDataChars)) as Dictionary<string, object>;
        Dictionary<string, string> result = new Dictionary<string, string>();
        if (data != null)
        {
            foreach (KeyValuePair<string, object> keyValuePair in data)
            {
                result.Add(keyValuePair.Key, keyValuePair.Value == null ? "" : keyValuePair.Value.ToString());
            }
        }
        else
        {
            Debug.LogError("Wrong Data Json");
        }
        return result;
    }

    public void Button_CreateTournament()
    {
        FB.CreateTournament(
            int.Parse(InitialScore.text),
            Title.text,
            Image.text,
            SortOrder.options[SortOrder.value].text,
            ScoreFormat.options[ScoreFormat.value].text,
            ConvertDataToDict(Data.text),
            CallbackCreateTournament
        );
    }

    private void CallbackCreateTournament(ITournamentResult result)
    {
        if (result.Error != null)
        {
            Logger.DebugErrorLog(result.Error);
        }
        else
        {
            Logger.DebugLog(result.RawResult);
        }
    }

    public void Button_PostSessionScore()
    {
        FB.PostSessionScore(int.Parse(Score.text), CallbackPostSessionScore);
    }

    private void CallbackPostSessionScore(ISessionScoreResult result)
    {
        if (result.Error != null)
        {
            Logger.DebugErrorLog(result.Error);
        }
        else
        {
            Logger.DebugLog(result.RawResult);
        }
    }

    public void Button_PostTournamentScore()
    {
        FB.PostTournamentScore(int.Parse(Score.text), CallbackPostTournamentScore);
    }

    private void CallbackPostTournamentScore(ITournamentScoreResult result)
    {
        if (result.Error != null)
        {
            Logger.DebugErrorLog(result.Error);
        }
        else
        {
            Logger.DebugLog(result.RawResult);
        }
    }

    public void Button_ShareTournament()
    {
        FB.ShareTournament(int.Parse(Score.text), ConvertDataToDict(ShareData.text), CallbackShareTournament);
    }

    private void CallbackShareTournament(ITournamentScoreResult result)
    {
        if (result.Error != null)
        {
            Logger.DebugErrorLog(result.Error);
        }
        else
        {
            Logger.DebugLog(result.RawResult);
        }
    }

    public void Button_GetTournament()
    {
        FB.GetTournament(CallbackGetTournament);
    }

    private void CallbackGetTournament(ITournamentResult result)
    {
        if (result.Error != null)
        {
            Logger.DebugErrorLog(result.Error);
        }
        else
        {
            Logger.DebugLog(result.RawResult);
        }
    }
}
