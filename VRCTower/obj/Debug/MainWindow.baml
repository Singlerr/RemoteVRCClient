   M S B A M L   `   `   `  ����  VRCTower    VRCTower.MainWindow   5   	   D @ModernWpf, Version=0.9.4.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.4.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � WindowStartupLocation$  CenterScreen=�5   	   0  *ModernWpf.Controls.Primitives.WindowHelper   UseModernWindowStyle .  � Title$ 
MainWindow��5
   	   $	��450��6   $	��800��6)   .��5      � .+�5   
   �� $��Center=�6   $	��308��61   
���   �$��Center=�6M   $	��638��6h   �� 5   
   $"��VRChat에 로그인하세요��6   $
��Left=�6)   $��	81,19,0,0q�6D   $	��Top=�6W   $��30(�5      �� 5   
   -   5        $��_idText��5      $��0��6!   $
��Left=�6.   $��107,148,0,0q�6I    �� TextWrapping$
 Wrap=�6^   $�� ��5      $	��Top=�5      $	��203��6*   $��12��66   �� 5   
   -   5        $��	로그인��5      $��2��6   $��107,233,0,0q�6,   $	��Top=�6A   $��34��6Y   $	��203��6e   5      �� 5   
   $��VRChat ID로 로그인��6   $
��Left=�6)   $��107,124,0,0q�6D   $	��Top=�6Y   �� 5   
   -   5        $��	로그인��5      $��455,148,0,0q�6   $	��Top=�64   $��75��6L   $	��181��6X   5      �� 5   
   $��Steam으로 로그인��6   $
��Left=�6&   $��455,124,0,0q�6A   $	��Top=�6V   G� 5   
   -   5        $��_passwordText��5      $��1��6+   $
��Left=�68   $��107,185,0,0q�6S   $	��Top=�5      $	��203��5      $��32��6"   � 5   
   -   5        $��	_progress��5      $��Center=�6'   $y�Hidden=�6D   $��24��6X   $��	0,316,0,0q�6d   $	��Top=�5      $	��586��6.    � IsIndeterminate . 6:   �� 5   
   $
��Left=�6   $��119,156,0,0q�6/   $
��Wrap=�6D   $��ID��6X   $	��Top=�5      ��5       �� 5!       �� 
TargetType" ��6   .�5"      ��  �� Property � y� �� Value$ Hidden=�63   �� 5#       � ��
 �����63    �� Triggers 5$      �� 5%      	 �� Binding	 6&   �� *Text+
 �� ElementName$
 _idText�� �� Value  6T   .a�5&      ��  � y�$ Visible=�6;   5'      5)      5+      �� 5,   
   $
��Left=�6   $��119,193,0,0q�6/   $
��Wrap=�6D   $��PW��6X   $	��Top=�5-      ��5.      �� 5/      " ��6   .�50      ��  � y�$ Hidden=�63   �� 51       � ��
 �����63    52      �� 53      	 6&   �� *Text+$
 _passwordText��  6Z   .a�54      ��  � y�$ Visible=�6;   55      57      59      5;      5<      