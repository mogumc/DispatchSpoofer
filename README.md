# DispatchSpoofer
Proxy + Gateserver spoofer for an anime game with stars and rails

# Features
- Capable of redirecting traffic to a specified host (currently IP and port are hardcoded)
- Reads a hotfix from given json data and spoofs the urls in decoded protobuf message
- Future-proof code

# Usage 
- Compile the code via Visual Studio 2022 or via the command:
```bash
dotnet compile
```
- Bring your hotfix.json to the `DispatchSpoofer\bin\Debug\net8.0`, you can get one [here](https://github.com/Hiro420/FetchHotfixCS)
- Run the DispatchSpoofer.exe, trust the certificate if asked (it's needed to decrypt SSL)

# I DO NOT CLAIM ANY RESPONSIBILITY FOR ANY USAGE OF THIS SOFTWARE, THE SOFTWARE IS MAKE 100% FOR EDUCATIONAL PURPOSES ONLY

Copyright© Hiro420