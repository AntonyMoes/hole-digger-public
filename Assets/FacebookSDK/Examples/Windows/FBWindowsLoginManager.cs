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

using System.Collections;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FBWindowsLoginManager : MonoBehaviour
{
    public FBWindowsLogsManager Logger;
    public InputField Permissions;
    public RawImage UserImage;
    public Text UserName;


    public void LogInReadButton()
    {
        if (FB.IsInitialized)
        {
            FB.LogInWithReadPermissions(Permissions.text.Split(','), AuthCallback);
        }
        else
        {
            Logger.DebugWarningLog("Not Init");
        }
    }

    public void LogInPublishButton()
    {
        if (FB.IsInitialized)
        {
            FB.LogInWithPublishPermissions(Permissions.text.Split(','), AuthCallback);
        }
        else
        {
            Logger.DebugWarningLog("Not Init");
        }
    }

    public void LogOutButton()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            Logger.DebugLog("Logged out");
        }
        else
        {
            Logger.DebugWarningLog("Login First");
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (result.Error != null)
        {
            Logger.DebugErrorLog("Error: " + result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Logger.DebugLog("aToken.UserId: " + aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions)
                {
                    Logger.DebugLog("perm: " + perm);
                }
                //get current profile data
                GetCurrentProfile();
            }
            else
            {
                Logger.DebugWarningLog("User cancelled login");
            }
        }
    }

    public void GetCurrentProfile()
    {
        Logger.DebugLog("Getting current user profile ...");

        FB.CurrentProfile((IProfileResult result) =>
        {
            if (result.Error != null)
            {
                Logger.DebugErrorLog(result.Error);
            }
            else
            {
                Logger.DebugLog("UserID: " + result.CurrentProfile.UserID);
                Logger.DebugLog("Name: " + result.CurrentProfile.Name);
                Logger.DebugLog("First Name: " + result.CurrentProfile.FirstName);
                Logger.DebugLog("Email: " + result.CurrentProfile.Email);
                Logger.DebugLog("ImageURL: " + result.CurrentProfile.ImageURL);
                GetUserLocale();

                UserName.text = result.CurrentProfile.Name + " " + result.CurrentProfile.LastName;
                if (result.CurrentProfile.ImageURL != "" && result.CurrentProfile.ImageURL != null)
                {
                    StartCoroutine(LoadPictureFromUrl(result.CurrentProfile.ImageURL, UserImage));
                }
            }
        });
    }

    public void GetUserLocale()
    {
        FB.GetUserLocale((ILocaleResult result) =>
        {
            if (result.Error != null)
            {
                Logger.DebugErrorLog("Error getting user locale: " + result.Error);
            }
            else
            {
                Logger.DebugLog("User Locale: " + result.Locale);
            }
        });
    }

    IEnumerator LoadPictureFromUrl(string url, RawImage itemImage)
    {
        Texture2D UserPicture = new Texture2D(32, 32);

        WWW www = new WWW(url);
        yield return www;

        www.LoadImageIntoTexture(UserPicture);
        www.Dispose();
        www = null;

        itemImage.texture = UserPicture;
    }
}
