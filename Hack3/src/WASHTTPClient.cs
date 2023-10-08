using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using WAS;
using Wizards.Arena.Promises;
using Wizards.MDN;

// Token: 0x02000592 RID: 1426
public class WASHTTPClient
{
	// Token: 0x1700064E RID: 1614
	// (get) Token: 0x06003749 RID: 14153 RVA: 0x0013CE85 File Offset: 0x0013B085
	// (set) Token: 0x0600374A RID: 14154 RVA: 0x0013CE8C File Offset: 0x0013B08C
	public static string ClientID { get; private set; }

	// Token: 0x1700064F RID: 1615
	// (get) Token: 0x0600374B RID: 14155 RVA: 0x0013CE94 File Offset: 0x0013B094
	// (set) Token: 0x0600374C RID: 14156 RVA: 0x0013CE9B File Offset: 0x0013B09B
	public static string ClientSecret { get; private set; }

	// Token: 0x17000650 RID: 1616
	// (get) Token: 0x0600374D RID: 14157 RVA: 0x0013CEA3 File Offset: 0x0013B0A3
	// (set) Token: 0x0600374E RID: 14158 RVA: 0x0013CEAA File Offset: 0x0013B0AA
	public static EnvironmentType ClientEnvironment { get; private set; }

	// Token: 0x0600374F RID: 14159 RVA: 0x0013CEB2 File Offset: 0x0013B0B2
	public static void Init(string clientId, string clientSecret, EnvironmentType clientEnv)
	{
		WASHTTPClient.ClientID = clientId;
		WASHTTPClient.ClientSecret = clientSecret;
		WASHTTPClient.ClientEnvironment = clientEnv;
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x0013CEC8 File Offset: 0x0013B0C8
	private static string GetErrorMessage(MdnWebRequest www)
	{
		string text;
		if (!string.IsNullOrWhiteSpace(www.responseText))
		{
			text = www.responseText;
		}
		else if (www.isNetworkError)
		{
			text = "Network Error";
		}
		else
		{
			text = www.error;
		}
		return text;
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x0013CF04 File Offset: 0x0013B104
	internal static WebPromise Login(string username, string password)
	{
		string text = "https://api.platform.wizards.com/auth/oauth/token";
		KeyValuePair<string, string> basicAuthHeader = WASHTTPClient.GetBasicAuthHeader(WASHTTPClient.ClientID, WASHTTPClient.ClientSecret);
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { basicAuthHeader.Key, basicAuthHeader.Value } };
		string content = WASHTTPClient.GetContent(new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("grant_type", "password"),
			new KeyValuePair<string, string>("username", username),
			new KeyValuePair<string, string>("password", password)
		});
		return WebPromise.PostForm(text, dictionary, content);
	}

	// Token: 0x06003752 RID: 14162 RVA: 0x0013CF89 File Offset: 0x0013B189
	internal static WebPromise RegisterAsFullAccount(string input, string language)
	{
		return WASHTTPClient.Register("accounts/register", input, language);
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x0013CF97 File Offset: 0x0013B197
	internal static WebPromise RegisterAsSocialAccount(string input, string language)
	{
		return WASHTTPClient.Register("accounts/persona/register", input, language).WithLogging();
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0013CFAC File Offset: 0x0013B1AC
	internal static WebPromise Register(string path, string input, string language)
	{
		string text = "https://api.platform.wizards.com/" + path;
		KeyValuePair<string, string> basicAuthHeader = WASHTTPClient.GetBasicAuthHeader(WASHTTPClient.ClientID, WASHTTPClient.ClientSecret);
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ basicAuthHeader.Key, basicAuthHeader.Value },
			{ "Accept-Language", language }
		};
		return WebPromise.PostJson(text, dictionary, input);
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x0013D004 File Offset: 0x0013B204
	internal static WebPromise LoginWithRefreshToken(string token)
	{
		string text = "https://api.platform.wizards.com/auth/oauth/token";
		KeyValuePair<string, string> basicAuthHeader = WASHTTPClient.GetBasicAuthHeader(WASHTTPClient.ClientID, WASHTTPClient.ClientSecret);
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { basicAuthHeader.Key, basicAuthHeader.Value } };
		string content = WASHTTPClient.GetContent(new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("grant_type", "refresh_token"),
			new KeyValuePair<string, string>("refresh_token", token)
		});
		return WebPromise.PostForm(text, dictionary, content);
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x0013D078 File Offset: 0x0013B278
	internal static WebPromise LoginWithSocialToken(string type, string token)
	{
		string text = "https://api.platform.wizards.com/auth/social/" + type + "/token";
		KeyValuePair<string, string> basicAuthHeader = WASHTTPClient.GetBasicAuthHeader(WASHTTPClient.ClientID, WASHTTPClient.ClientSecret);
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { basicAuthHeader.Key, basicAuthHeader.Value } };
		string text2 = JsonConvert.SerializeObject(new
		{
			social_token = token
		});
		return WebPromise.PostJson(text, dictionary, text2).WithLogging();
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x0013D0D7 File Offset: 0x0013B2D7
	internal static WebPromise GetLinkedAccounts(string accessToken)
	{
		return WASHTTPClient.PlatformGet("accounts/socialidentities", string.Empty, accessToken);
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x0013D0E9 File Offset: 0x0013B2E9
	internal static IEnumerator ForgotPassword(string input, Action<string> resultCallback, Action<WASHTTPClient.WASError> errorCallback)
	{
		string text = "accounts/forgotpassword";
		string text2 = "https://api.platform.wizards.com/" + text;
		KeyValuePair<string, string> basicAuthHeader = WASHTTPClient.GetBasicAuthHeader(WASHTTPClient.ClientID, WASHTTPClient.ClientSecret);
		MdnWebRequest www = new MdnWebRequest(text2, "POST");
		www.SetBody(input);
		www.SetRequestHeader("Content-Type", "application/json");
		www.SetRequestHeader(basicAuthHeader.Key, basicAuthHeader.Value);
		yield return www.Send();
		if (www.isNetworkError || www.isHttpError)
		{
			if (errorCallback != null)
			{
				errorCallback(new WASHTTPClient.WASError((int)www.responseCode, WASHTTPClient.GetErrorMessage(www)));
			}
		}
		else if (resultCallback != null)
		{
			resultCallback(www.responseText);
		}
		yield break;
	}

	// Token: 0x06003759 RID: 14169 RVA: 0x0013D106 File Offset: 0x0013B306
	internal static IEnumerator ValidateUsername(string input, Action<string> resultCallback, Action<WASHTTPClient.WASError> errorCallback)
	{
		string text = "accounts/moderate";
		string text2 = "https://api.platform.wizards.com/" + text;
		KeyValuePair<string, string> basicAuthHeader = WASHTTPClient.GetBasicAuthHeader(WASHTTPClient.ClientID, WASHTTPClient.ClientSecret);
		MdnWebRequest www = new MdnWebRequest(text2, "POST");
		www.SetBody(input);
		www.SetRequestHeader("Content-Type", "application/json");
		www.SetRequestHeader(basicAuthHeader.Key, basicAuthHeader.Value);
		yield return www.Send();
		if (www.isNetworkError || www.isHttpError)
		{
			if (errorCallback != null)
			{
				errorCallback(new WASHTTPClient.WASError((int)www.responseCode, WASHTTPClient.GetErrorMessage(www)));
			}
		}
		else if (resultCallback != null)
		{
			resultCallback(www.responseText);
		}
		yield break;
	}

	// Token: 0x0600375A RID: 14170 RVA: 0x0013D124 File Offset: 0x0013B324
	internal static WebPromise PlatformPost(string path, string input, string language, string accessToken, Dictionary<string, string> header = null)
	{
		if (header == null)
		{
			header = new Dictionary<string, string>();
		}
		header.Add("Authorization", "Bearer " + accessToken);
		header.Add("Accept-Language", language);
		return WebPromise.PostJson("https://api.platform.wizards.com/" + path, header, input);
	}

	// Token: 0x0600375B RID: 14171 RVA: 0x0013D173 File Offset: 0x0013B373
	internal static WebPromise GetPurchaseToken(string input, string language, string accessToken)
	{
		return WASHTTPClient.PlatformPost("xsollaconnector/client/token", input, language, accessToken, null);
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x0013D184 File Offset: 0x0013B384
	private static WebPromise PlatformGet(string path, string language, string accessToken)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{
				"Authorization",
				"Bearer " + accessToken
			},
			{ "Accept-Language", language }
		};
		return WebPromise.Get("https://api.platform.wizards.com/" + path, dictionary);
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x0013D1CA File Offset: 0x0013B3CA
	internal static WebPromise GetProfileToken(string language, string accessToken)
	{
		return WASHTTPClient.PlatformGet("xsollaconnector/client/profile", language, accessToken);
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x0013D1D8 File Offset: 0x0013B3D8
	internal static WebPromise GetProfile(string accessToken)
	{
		return WASHTTPClient.PlatformGet("profile", string.Empty, accessToken);
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x0013D1EA File Offset: 0x0013B3EA
	internal static WebPromise GetAllEntitlementsByReceiptIdAndSource(string receiptId, string source, string language, string accessToken)
	{
		return WASHTTPClient.PlatformGet("entitlements/source/" + source + "/receipt/" + receiptId, language, accessToken);
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x0013D204 File Offset: 0x0013B404
	internal static WebPromise TryValidateReceipt(string store, string input, string language, string accessToken)
	{
		return WASHTTPClient.PlatformPost("receiptverification/verify/" + store, input, language, accessToken, null);
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x0013D21A File Offset: 0x0013B41A
	internal static Promise<string> RedeemCode(string language, string code, string accessToken)
	{
		if (!WASHTTPClient.ValidateUriSafeString(code))
		{
			return new SimplePromise<string>(new Error(404, "CODE NOT FOUND", null), ErrorSource.Unknown);
		}
		return WASHTTPClient.PlatformGet("redemption/code/" + code, language, accessToken);
	}

	// Token: 0x06003762 RID: 14178 RVA: 0x0013D24D File Offset: 0x0013B44D
	internal static IEnumerator GetStoreItems(string language, string currency, string accessToken, Action<string> resultCallback, Action<WASHTTPClient.WASError> errorCallback)
	{
		string text = "xsollaconnector/client/skus";
		if (!string.IsNullOrEmpty(currency))
		{
			text = text + "?currency=" + currency;
		}
		MdnWebRequest www = MdnWebRequest.Get("https://api.platform.wizards.com/" + text);
		www.SetRequestHeader("Authorization", "Bearer " + accessToken);
		www.SetRequestHeader("Accept-Language", language);
		yield return www.Send();
		if (www.isNetworkError || www.isHttpError)
		{
			if (errorCallback != null)
			{
				errorCallback(new WASHTTPClient.WASError((int)www.responseCode, WASHTTPClient.GetErrorMessage(www)));
			}
		}
		else if (resultCallback != null)
		{
			resultCallback(www.responseText);
		}
		yield break;
	}

	// Token: 0x06003763 RID: 14179 RVA: 0x0013D279 File Offset: 0x0013B479
	public static WebPromise InitSteamPurchase(string accessToken, string language, string body)
	{
		return WASHTTPClient.PlatformPost("purchase/steam/initiate", body, language, accessToken, null).WithLogging();
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x0013D28E File Offset: 0x0013B48E
	public static WebPromise LinkSocialAccount(string socialType, string socialToken, string accessToken)
	{
		return WASHTTPClient.RequestLink(socialType, socialToken, accessToken, "none");
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x0013D29D File Offset: 0x0013B49D
	public static WebPromise ResolveConflict(string socialType, string socialToken, string accessToken, ConflictingPersona personaToKeep)
	{
		return WASHTTPClient.RequestLink(socialType, socialToken, accessToken, personaToKeep.personaType);
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x0013D2B0 File Offset: 0x0013B4B0
	private static WebPromise RequestLink(string socialType, string socialToken, string accessToken, string forcePersonaType = "none")
	{
		string text = "auth/social/" + socialType + "/link";
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "social_token", socialToken },
			{ "force_on_persona", forcePersonaType }
		};
		return WASHTTPClient.PlatformPost(text, JsonConvert.SerializeObject(dictionary), string.Empty, accessToken, null).WithLogging();
	}

	// Token: 0x06003767 RID: 14183 RVA: 0x0013D304 File Offset: 0x0013B504
	public static WebPromise GetConflictingPersonas(string socialType, string socialToken, string accessToken)
	{
		string text = "auth/social/" + socialType + "/conflictInfo";
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { "social_token", socialToken } };
		return WASHTTPClient.PlatformPost(text, JsonConvert.SerializeObject(dictionary), string.Empty, accessToken, null).WithLogging();
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x0013D34A File Offset: 0x0013B54A
	public static WebPromise CancelLinking(string socialType, string accessToken)
	{
		return WASHTTPClient.PlatformPost("auth/social/" + socialType + "/unlink", string.Empty, string.Empty, accessToken, null).WithLogging();
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x0013D374 File Offset: 0x0013B574
	private static bool ValidateUriSafeString(string str)
	{
		return str != null && !str.Contains("..") && !str.Contains("\\") && !str.Contains("/") && !str.Contains(" ") && !str.Contains("\t") && !str.Contains("\r") && !str.Contains("\n");
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x0013D3E4 File Offset: 0x0013B5E4
	public static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		bool flag = true;
		if (sslPolicyErrors != SslPolicyErrors.None)
		{
			for (int i = 0; i < chain.ChainStatus.Length; i++)
			{
				if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
				{
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					if (!chain.Build((X509Certificate2)certificate))
					{
						flag = false;
						break;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x0013D46C File Offset: 0x0013B66C
	private static string GetContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
	{
		if (nameValueCollection == null)
		{
			throw new ArgumentNullException("nameValueCollection");
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> keyValuePair in nameValueCollection)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append('&');
			}
			stringBuilder.Append(WASHTTPClient.Encode(keyValuePair.Key));
			stringBuilder.Append('=');
			stringBuilder.Append(WASHTTPClient.Encode(keyValuePair.Value));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x0013D508 File Offset: 0x0013B708
	private static string Encode(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			return string.Empty;
		}
		return Uri.EscapeDataString(data).Replace("%20", "+");
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x0013D530 File Offset: 0x0013B730
	private static KeyValuePair<string, string> GetBasicAuthHeader(string clientId, string clientSecret)
	{
		string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientId, clientSecret)));
		string text2 = string.Format("Basic {0}", text);
		return new KeyValuePair<string, string>("Authorization", text2);
	}

	// Token: 0x04002922 RID: 10530
	public const string BaseUri = "https://api.platform.wizards.com/";

	// Token: 0x04002923 RID: 10531
	public const string JsonMediaType = "application/json";

	// Token: 0x04002924 RID: 10532
	public const string FormMediaType = "application/x-www-form-urlencoded";

	// Token: 0x04002925 RID: 10533
	public const string ContentType = "Content-Type";

	// Token: 0x04002926 RID: 10534
	public const string AcceptLanguage = "Accept-Language";

	// Token: 0x02002984 RID: 10628
	public class WASError
	{
		// Token: 0x0600B136 RID: 45366 RVA: 0x002D3BB0 File Offset: 0x002D1DB0
		public WASError(int code, string message)
		{
			this.Code = code;
			this.Message = message;
		}

		// Token: 0x0400CF9F RID: 53151
		public int Code;

		// Token: 0x0400CFA0 RID: 53152
		public string Message;
	}
}
