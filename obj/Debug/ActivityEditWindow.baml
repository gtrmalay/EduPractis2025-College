   M S B A M L   `   `   `  ����  EventsWPF_Practic*    $EventsWPF_Practic.ActivityEditWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$7  1Редактирование активности��5   	   $	��300��6+   $	��300��68   
���   �.��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   .+�5   
   �� $��0��6   
������� �� CornerRadius$ 8��61    �� Padding$ 10q�6B   $��0,0,0,15q�6O   ���#E0E0E0$��1q�6w   ._�5      �� .+�5      �� $��	День:��6   ���#333333$��0,0,0,2q�6>   �� 5      -   5         ��Name$ DayNumberBox��5      $��0,0,0,5q�60   .��6A    � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '��       �� 5      " ��6    .�5      ��  � ��
 �����6;   �� 5       � �� �#E0E0E06<   �� 5       � ��$ 1q�6@   �� 5       � ��$ 5q�68   5      5      .��6)   5      �� 5      $��Время начала:��6   ���#333333$��0,0,0,2q�6F   �� 5      -   5        $ StartTimeBox��5      $��0,0,0,5q�60   .��6A    5      ��%�   '��       �� 5      " ��6    .�5      ��  � ��
 �����6;   �� 5       � �� �#E0E0E06<   �� 5        � ��$ 1q�6@   �� 5!       � ��$ 5q�68   5"      5#      .��6)   5$      �� 5%      $��Мероприятие:��6   ���#333333$��0,0,0,2q�6E   �� 5&      -   5        	 ��Name$	 EventComboBox��5&      $��0,0,0,5q�62    5'      ��%�   '��       �� 5(      " ��6    .�5)      ��  � ��
 �����6;   �� 5*       � �� �#E0E0E06<   �� 5+       � ��$ 1q�6@   �� 5,       � ��$ 5q�68   5-      5.      5/      50      51      �� 53   
   $��2��6   
 �� Orientation$
 
Horizontal=�6"   $��Right=�6;   .+�54      �� -   5        $��Сохранить��54      $��80��6)   $��0,0,10,0q�64   
���� ��
�������$��0q�55   <   $
��10,5q�6P    56       �� 
TargetType �� Triggers P� Property P� Value��%[  '��       �� 57      " ��6   .�58      ��  � �� 59      �� 5:   "   " ��62   .R�5;   &   �� #��z��6-   $ 4��6W   # z��6h   ._�5<   *   �� $��Center=�6;   $��Center=�6X   5=   '   5>   #   5@       5A      P� 5B       �@J�IsMouseOver . . �5C   "   ��  � �� �#1976D26?   5D      5F      5G      5H      �� 5I      -   5        $��Отмена��5I      $��80��6&   
���� ��
�������$��0q�5J   <   $
��10,5q�6P    5K      ��%[  '��       �� 5L      " ��6   .�5M      ��  � �� 5N      �� 5O   "   " ��62   .R�5P   &   �� #��z��6-   $ 4��6W   # z��6h   ._�5Q   *   �� $��Center=�6;   $��Center=�6X   5R   '   5S   #   5U       5V      P� 5W       �@J�IsMouseOver . . �5X   "   ��  � �� �#1976D26?   5Y      5[      5\      5]      5^      5_      5`      