# hacking the api

- [x] set up the environment, kali, insomnia, burp, python, vscode
- [x] perform auth request towards the api, foothold!
- [x] fuzz the api for documentation
- [x] study the race condition lab from PortSwigger
- [ ] Hook the `WebPromise.cs` constructor to use local burp proxy and ignore ssl validation
- [ ] study the purchase flow
- [ ] Find the endpoint with parameters that is ripe for exploitation
- [ ] Write exploit, conceptually
- [ ] Test Exploit



# Connection information
accountSystemId: N8QFG8NEBJ5T35FB
accountSystemSecret: 	

The above connection information seems to be the same across restarts of the client.


# How is the request formated?
Following callstack is used

```csharp
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
```

The `GetContent` function is setting POST body parameters using `key=value&key2=value2` etc.

```csharp
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
				stringBuilder.Append('&'); // which means if this is the second parameter in the loop, add &
			}
			stringBuilder.Append(WASHTTPClient.Encode(keyValuePair.Key));
			stringBuilder.Append('=');
			stringBuilder.Append(WASHTTPClient.Encode(keyValuePair.Value));
		}
		return stringBuilder.ToString();
```

```csharp
	// Token: 0x0600376D RID: 14189 RVA: 0x0013D530 File Offset: 0x0013B730
	private static KeyValuePair<string, string> GetBasicAuthHeader(string clientId, string clientSecret)
	{
		string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientId, clientSecret)));
		string text2 = string.Format("Basic {0}", text);
		return new KeyValuePair<string, string>("Authorization", text2);
	}
```


# Example Captured Requests:
## https://api/auth/oauth/token
```Yaml
POST https://api.platform.wizards.com/auth/oauth/token:
Authorization: Basic TjhRRkc4TkVCSjVUMzVGQjpWTUsxUkU4WUs2WVI0RUFCSlU5MQ==
Content-Type: application/x-www-form-urlencoded; charset=utf-8
grant_type=refresh_token&refresh_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IjM0NmM4YTY1NTBlZGI5MDRjM2IyNWI3ODlmOTllNjU3ODA4MGJiOTUiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOiJOOFFGRzhORUJKNVQzNUZCIiwiZXhwIjoxNjk3OTY5Mzc0LCJpYXQiOjE2OTY3NTk3NzQsImlzcyI6IklOTVZERDJISDVES0pISFNGR0NLR0dPU0NZIiwic3ViIjoiTEI0WFNLNDVGWkhPWkVMNDZPM1FWV0EzS0EiLCJ3b3RjLWRvbW4iOiJ3aXphcmRzIiwid290Yy1zY3BzIjpbImZpcnN0LXBhcnR5Il0sIndvdGMtZmxncyI6MSwid290Yy1wZGdyIjoiQzNWT0FONjZXTkNGRkFDQ001UkZXM0RXQk0iLCJ3b3RjLXNvY2wiOnt9LCJ3b3RjLWNuc3QiOjB9.WhF404LHWSnjDYgoyTcsAOfdbwhzkpW7-5mqFKIL7-pt9rCLNE6pO208ZanG54yz0zobrOwgHOMOOYryqD1MBuShIcyDsB4OW6QJHXosxrCY_0-E0uY8i_-KChdRfS3F086GhM19H1yMDYqlgdtTnbQhyrPSObq8Y5NJJMfS7Ej-PSbVKkZFdyCmNto9CXOCrLxOYwfZw_TjW_90oWGEHDHv4snpsXPrjzAiTzAAR7uHYBHIex0yq_A_SOHObMrVaB03j2TTq8pkNSnj7HL_FiK6jkFCMR-NnXe5vmXPIFHvvqLfVpeWcPkPhGcdcAiWXNJFVCI7PhxR6i0VUjpm9w
```

## https://api/profile

```yaml
GET https://api.platform.wizards.com/profile:
Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6ImQwNGMxYzYxNTkwNDBmZGRhN2FlYjI0ODViOWU0MTBlZDM0ZDJkMDgiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOiJOOFFGRzhORUJKNVQzNUZCIiwiZXhwIjoxNjk2NzYwNzgxLCJpYXQiOjE2OTY3NTk4MjEsImlzcyI6IklOTVZERDJISDVES0pISFNGR0NLR0dPU0NZIiwic3ViIjoiTEI0WFNLNDVGWkhPWkVMNDZPM1FWV0EzS0EiLCJ3b3RjLW5hbWUiOiJQZXBwYVBFRUVFRyM5MTI1OSIsIndvdGMtZG9tbiI6IndpemFyZHMiLCJ3b3RjLWdhbWUiOiJhcmVuYSIsIndvdGMtZmxncyI6MSwid290Yy1yb2xzIjpbIk1ETkFMUEhBIl0sIndvdGMtcHJtcyI6W10sIndvdGMtc2NwcyI6WyJmaXJzdC1wYXJ0eSJdLCJ3b3RjLXBkZ3IiOiJDM1ZPQU42NldOQ0ZGQUNDTTVSRlczRFdCTSIsIndvdGMtc2d0cyI6W10sIndvdGMtc29jbCI6e30sIndvdGMtY25zdCI6MH0.Ep7bDMq8cNku-9yycsqZ77dBV1NJyfYPid7X8xk_KPpyaDJrVprjVioJDRswV3hzGqIvn1f1TzfE0LEHYsj9yGDC0kHV0g1FoF9wWjeRq7GG4m7RMEjF3Qv82f4z04jSD4Qk9KTfAm6tZS5R0ZZn9pelnD22FiWjR551bpKhsMqhJkz9VOPZIGDVjhrMiTNCSJ9PdyHc2O-Imm3orQUuqjnVwYdqGWui3NQ8NdofFtK6601TRniRsZN_EjftbQTiaTT1782s7ngkQ1ZRoNAtVZU0zKLS_lchi70XP09Ojfrms9AtJ-vb2UUTKMO88AovF73NmmOBGNW907sHW-4mVQ
```
# how to pwn the site

This is from the lab https://portswigger.net/web-security/race-conditions/lab-race-conditions-limit-overrun

    - Remove the discount code from your cart.

    - In Repeater, send the group of requests again, but this time in parallel, effectively applying the discount code multiple times at once. For details on how to do this, see Sending requests in parallel.

    - Study the responses and observe that multiple requests received a response indicating that the code was successfully applied. If not, remove the code from your cart and repeat the attack.

    - In the browser, refresh your cart and confirm that the 20% reduction has been applied more than once, resulting in a significantly cheaper order.

# Prove the concept

    - Remove the applied codes and the arbitrary item from your cart and add the leather jacket to your cart instead.

    - Resend the group of POST /cart/coupon requests in parallel.

    Refresh the cart and check the order total:
        If the order total is still higher than your remaining store credit, remove the discount codes and repeat the attack.
        If the order total is less than your remaining store credit, purchase the jacket to solve the lab.

# Hooking the Constructor of `WebPromise.cs`

After reading a lot of code I found that the class that is doing many of the requests from the client is called `WebPromise.cs`.

Here is the code I think I need to hook

```csharp
namespace Wizards.Arena.Promises
{
	// Token: 0x02000014 RID: 20
	public class WebPromise : Promise<string>
	{
		// Token: 0x0400002F RID: 47
		private static readonly HttpClient client = new HttpClient
		{
			Timeout = TimeSpan.FromMilliseconds(5000.0)
		};
		...
	}
}
```

need to become 