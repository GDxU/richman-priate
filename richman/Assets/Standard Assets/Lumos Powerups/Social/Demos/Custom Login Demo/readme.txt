 --- Lumos Social Demo (Custom Login) ---
 
 Overview:
 Social services like GameCenter and Google Play already have users that have 
 already registered accounts for those platforms. Your users will not have 
 registered with Lumos though, so they must go through some sort of 
 registration process. We offer various ways to do this so you can make the 
 registration process as seemless as you'd like. For many developers, your 
 players won't even know they were registered.
 
 Registering with Lumos Social is game specific, meaning the accounts created 
 with your game will only ever exist on your game.
 
 1. LumosSocialDemoCustomLogin.cs
 This class demonstrates the three different ways you could implement user 
 login and registration. Factors to consider when choosing which 
 login/registration your game will use are as follows:
  
 - You want to display leaderboards with usernames (requires username)
 - Cross platform/device login matters (requires username and password)
 - You want friend lists (typically requires username)
 - You want to send a welcome email (requires email address)
 - You want to support the "forgot password" feature (requires email address)
 
 Read the comments in this class to see where these factors fall into 
 various registration and login approaches.
 
 2. LumosSocialDemoCustomLoginGUI
 This class demonstrates various ways to use the LumosSocialDemoCustomLogin.cs 
 class. Run the demo to explore different scenerios that take the factors 
 above into account.
 