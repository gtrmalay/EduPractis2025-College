   M S B A M L   `   `   `  ����  EventsWPF_Practic'    !EventsWPF_Practic.ModeratorWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$#  Окно Модератора��5   	   $	��450��6!   $	��550��6.   
���   �.��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   
������� �� CornerRadius$ 8��61    �� Padding$ 10q�6B   $��0,0,0,15q�6O   ���#E0E0E0$��1q�6w   ._�5      �� -   5         ��Name$ GreetingTextBlock��5      $��18(�63   $��Medium"�6A   ���#3333336U   5      �� 5   
   $��1��6   
�������$ 8��61   $ 10q�6B   $��0,0,0,15q�6O   ���#E0E0E0$��1q�6w   ._�5      �� -   5         ��Name$ EventsComboBox��5      $��0,0,0,0q�6/    �� DisplayMemberPath$ 	EventName��5       �� SelectedValuePath$ EventID��65    � 	Resources 5      	 �� 
TargetType
 �� Property �� Value��%�   '��       �� 5      "	 ��6   .�5      �� 
 � ��
 �����67   �� 5      
 � �� �#E0E0E068   �� 5      
 � ��$ 1q�6<   �� 5      
 � ��$ 5q�64   5      5      5      5      �� 5    
   -   5         ��Name$ ActivitiesListView��5       $��2��6/   $��0,0,0,15q�6<   
����������#E0E0E0$��1q�6w    5!       �� Triggers P� Property P� Value��%E  '��       �� 5"      "	 ��6   .�5#      �� 
 � ��$ Stretch=�6C   �� 5$      
 � ��$ 10q�60    5%      P� 5&       �@J�IsMouseOver . . �5'      �� 
 � �� �#F0F0F06;   5(      P� 5)       �@��
IsSelected . . �5*      �� 
 � �� �#E3F2FD6;   5+      5-      5.       �� View 5/      �� 50      .P�51      �� $��Мероприятие��6%    �� Width$	 150��6:    �� DisplayMemberBinding 6F   �� *	EventName+�� 52      $��День��6%   $ 50��63    6>   �� *	DayNumber+53      55      �� 57   
   $��3��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   .+�58      �� -   5         ��Name$ RegisterActivityButton��58      $)��#Выбрать активность��65   $	��150��6R   $��0,0,10,0q�6^   
���� ��
�������$��0q�59   <   $
��10,5q�6P    5:       �� 
TargetType��%[  '��       �� 5;      "	 ��6   .�5<      �� 
 � �� 5=      �� 5>   "   " ��62   .R�5?   &   �� #��z��6-   $ 4��6W   # z��6h   ._�5@   *   �� $��Center=�6;   $��Center=�6X   5A   '   5B   #   5D       5E      P� 5F       �@J�IsMouseOver . . �5G   "   �� 
 � �� �#1976D26?   5H      5J      5K      5L      �� 5M      -   5        $ LogoutButton��5M      $��
Выйти��6+   $	��100��6;   
�������
���   �$��0q�5N   ;   $
��10,5q�6O    5O      ��%[  '��       �� 5P      "	 ��6   .�5Q      �� 
 � �� 5R      �� 5S   "   " ��62   .R�5T   &   �� #��z��6-   $ 4��6W   # z��6h   ._�5U   *   �� $��Center=�6;   $��Center=�6X   5V   '   5W   #   5Y       5Z      P� 5[       �@J�IsMouseOver . . �5\   "   �� 
 � �� �#D32F2F6?   5]      5_      5`      5a      5b      5c      5d      