﻿using System.Drawing;

namespace DispatchSpoofer;

public class Logger
{
	private string _name;

	public Logger(string name)
	{
		_name = name;
	}

	public static bool DoLogUselessInfo = false;

	public void ClearConsole()
	{
		Colorful.Console.ResetColor();
	}

	public void LogError(string message, bool isStopProgram = false)
	{
		LogMessage(message, LogColors.ERROR);
		if (isStopProgram)
		{
			Environment.Exit(0);
		}
	}

	public void LogWarning(string message)
	{
		LogMessage(message, LogColors.WARNING);
	}

	public void LogInfo(string message, bool isImportant = false)
	{
		if (DoLogUselessInfo || isImportant)
			LogMessage(message, LogColors.INFO);
	}

	public void LogSuccess(string message, bool isImportant = true)
	{
		if (DoLogUselessInfo || isImportant)
			LogMessage(message, LogColors.SUCCESS);
	}

	public void LogPacket(string message)
	{
		if (DoLogUselessInfo)
			LogMessage(message, LogColors.INFO);
	}

	private void LogMessage(string message, LogColors color)
	{
		Color colorCode = ColorTranslator.FromHtml($"#{(int)color:X6}");
		Colorful.Console.WriteLine($"[{_name}] {message}", colorCode);
		ClearConsole();
	}

	public void LogKazusa()
	{

		Color colorCode = ColorTranslator.FromHtml($"#{(int)LogColors.KAZUSA:X6}");
		Colorful.Console.WriteLine(Kazusa, colorCode);
		ClearConsole();
	}

	internal enum LogColors
	{
		None = 0xFFFFFF,
		INFO = 0x61AFEF,
		WARNING = 0xE5C07B,
		ERROR = 0xE06C75,
		SUCCESS = 0x98C379,
		KAZUSA = 0xFF00FF
	}

	internal readonly string Kazusa =
	@"                                 .                                                                                            
                           ::-:.:.-@#*==-     :+                                              :-..=:     .::..--:::-=...      
                           ..:::==:.:@%*++++=-:   :     ...               -+                   .:..:..    .-...-.....:=...=   
                           .:::.-==-:..@@#***+*+=:.    ..--=++******+=-.       @                --..-:.   .::..-.......:-..:. 
                            .:.:++--=--..@%+++**####**+*###**+=++**##%@@@@@#*-       .@          :=..--.   .:-..=........+... 
                             .:. *=====-:.@#######**++******#*@@@@#+**+++=+**%@@*....   =+        .:..--    -.-..-........-.. 
                             .::.+====--.%%*******++@@@%*#*+%*=--=+#%#*******+++#%@@*=-:    .     ..-...--:  ---...-....:.:-. 
                              ::.-*-=:.@@%#****+==@@*==+++#@++%%@@#++*******+********#**+=-.         :-..-::- ..::...=....-:. 
                              .:: =-.=@%#******+%@==***++%@=#**=*++@%****+#+++*+*******####*+-  :@      ...=-=  -:::...==--+. 
                               .:..:%%=*##*****@*=+++=@#+-@*+*+++#*++******#*++*+***+***+******+::+@@@@       ..  .-::......  
                                .  @#++*#*#**#%*+****@*+*+#%+*+#*******#****##++++**+*****++*******++*%@@@@#          ::-...  
                              :. .@#++=-*#**#*+****+%++***+@+*******%*+**#*****=+*+++**+****+***+++****+=+#@@@@@@@@*     ..   
                                 @#++=-#%%##*++#****%+**#*+%*********#*+**#***#@=+**+*+++******++++*****++++*#@@@@            
                               @#*++==@%.-###*#****#*+*****#*####***+@**+***#*+*@+-+****+**+***+***+**+++#@@@@: :  ::.        
                               .:**==%*-=#@+*#@@%##@*%%%@@#@##*#@@@%=#@##+***#*=#@@*%@@@@%%#######%%@@@@@@:   .+ ....=:.      
                              ::-#++#+--=%=-=--:-+%#*+-+-:.++----:: ++##@**+**##=*@#   .:*#%#%@@@@@%*:            .:...--.    
                              .-#=++#:-**=:--=====*--====+=#*+++=-:=#+%+-@+#**+**=*@@@@=     ..--:.  ...           .:-:...    
                             :-*=**##%#%%#****==-*+-:--*-=-##-:.=%@#*+***=%#*+++**=+:%%@@@@@%            :            -:=     
                             --**#+%**+%**####%%#@##*###=-:**-%%#****+%%**+*#*#++*+*#=#%=:*@@@@@@@@@@@@@                      
                            -:#****%**+#*#**#***+%*##*#%###@@+*******+*@-#**+*+#**++####@=-=+@%%+  @%.:                       
                           ..-#+++%+*=*+*=++++++#*+**+****+@@=*******++#@=+##+*#**+++#+#@#+=*@% #-:@=.#                       
                            :#++++#+*+#*%#++++++%=++++##+++=@+*******+=@#%*=%*++#**#+***#@=+*+@:%+:@%:+#                      
                           -=%=++***-#*=+++***++*++**#@@+***##+**+****=@:#*%:+@+=%*%@++#=%#=+%@-**-*@:=*                      
                           -*+++=#+*-@%.#*-**+**#%=++@ @-++# @==+***++*@@-+#@@@@@@*.:%#**%#=+=@+=*=:@:-+                      
                           :@-++=%=#-#- @#-+=-=#=@-++@  ==+@ @@=+++*++%.@@@%+    =@%@#*+*#@=++#@=#=.@-==:                     
                          -=%+++=%:%=@* @%.+=+*- @:.-@@@@-=@ %*+=+++==%.. @. .  @##@:%*#==@*++=@=#=:@+-==                     
                         .-+=%-+-@@* =  .@ =:=@:.@% :@  --@#  @@-+=+-=@+-   @@@@@%-+#-%##+*@+*-@+*=:@#-+==                    
                         --%=#-+-@.%@@@@.+=-.+@. #@= @.  +@@# :@--==--%@* .   @@@*=*@#%+**-@+*+@+*+=##-==-                    
                        :::=-*-+:@+=.@ @@* @@@@  .@@-@-   +@@  @@====-#-@ ...  @@*%@@*#@@@@@@@+@+*+=*#-==-                    
                        .=#*-#-+-@@:%. # @-     .    @#..        :.:-=+:@ ....  +@##@@@@@    @@%+++=+#-==#                    
                        .==@:#-+-@@#@   @@@ ........   .@@@@@@@@@@+=-..+@ .....   =@@    .#*  @@++==+*=#=%                    
                         @.*-*-+=%@ @. @%   ........ @        .@#@@@@@@@.+ ...... .   .%  .*: @@=*+=++-@:%                    
                        .*:*=*:*+*@ @*    ..........   ... :@= : -@  @=# + ...... +.. %*% .++  @-*+=#=:@.#.                   
                        -====+:#+*@ *  .................... %@+.=@ *%=-@ * ......% ..  :*..*  %@:++=#-=@.#.                   
                        ==#=+=:##+@ +.......................       @+=+@ # .....:.   %#%  +  @@*+++-@=+*:*.                   
                        +-=:+=.#@+@ =............................. -@=*# :.........@==. -. .@#%-#==-#====-                    
                        :.=+++.*@+@ =............................. .@+#* .......        .-#-@%@-*==+%=@:%.                    
                          +:-+.*@*@ -. ............................ @*@ :.....   @@@@@@#.  @@ =%-=-+**::::                    
                          =+=*.*@=@  @ ............................ @%- =..... @=:.:  .-. :@:=#*---#@=.=.@                    
                         ==-:#:+@: @ .+  ..   ..................... @@-=...  #=..:.::.*-  @@ +@::.:@+ .                       
                         -+*.@.-@-  =. @ .. @ ..................... #@    =+.%.....:..=  @@ -#-  :=-                          
                          +*.@+:@- .:...=..  +..................... +#=.+===:@ .::.-.-: @@  %*                                
                          -=.@::@: ::.:::+.......................   @+.:-==. +.=:...:- @@  =.                                 
                           : %:.@: ::.:::..+................    .+-:%.:=+ ...# -      -                                       
                             +@.@= .:.:::-..*............   =*===- =@.+....                                                   
                              @ @# .:...--...+=....+-:%*. @::::--: @         +                                                
                              @.@% .....-.-..+..-  .      @        ..-@@%@@@@@                                                
                              %*+@ ....:-  ..#=:#:        @ .=@@@@@=#%%@@@@@@                                                 
                               @ @ ......%                #:@@@@@ #@*                                                         
                               : @.:                       @*         .......  @                                              
                               -:                         -.:  .......          :.=@                                          
                             . ..                         %  ...       @@@@@@%%=.                                             
                              =-                                 @@@@@%+===++==-+*+***@@#*@@@@                                
                                                           .@@@@%*=--:...:=*####**+==:.                                       
                                                                                                                              
";
}