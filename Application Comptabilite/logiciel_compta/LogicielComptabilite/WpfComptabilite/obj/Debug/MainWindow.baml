   M S B A M L   `   `   `  ����J  FWpfComptabilite, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WpfComptabiliteWpfComptabilite  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	        WpfComptabilite.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   )localclr-namespace:WpfComptabilite  5   	   3System&clr-namespace:System;assembly=mscorlib  5   	     � Title$  Logiciel de comptabilité��5
   	   $	��450��6*   $	��800��67   $	��800��6C   $	��800��6R   $	��450��6a   $	��450��6q    � 	Resources 5          BooleanToVisibilityConverter��%   &	        �� 5   
   6'   5      .��5      � -   5         �Name$ 
principale��5      $��Stretch=�6   $��Stretch=�6=    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� ���	#FF74AC93$��Stretch=�6%   $��33��6C   ���	#FF74AC93$	��Top=�6b   $
��Auto��6z   �� 5   
   -   5         ��Name$ 
cmb_client��5      $ �� IsSynchronizedWithCurrentItem$
 TrueY�6J   $��	24,50,0,0q�6�   $��34��6�   $	��Top=�6�   $	��364��6�   $
��Left=�6�   ��6'   �� *
Lesclients+{�6o   �� *IsEnableLesClients+6�   � 5   
   -   5        $ 	infosUser��5      $	��386��6"   $��
416,33,0,0q�6/   .+�5      �� $	��185��6   ���	#FF74AC93$	��Top=�68   $
��Auto��6P   
�������6]   �� 5      
�������$	��201��6%   $��	0,185,0,0q�62   ���	#FF74AC93$	��Top=�6X   $
��Auto��6p   �� 5      $
��Wrap=�6   $!��Historique des transactions��6,   $	��Top=�6O   $
��Auto��6g   $��26��6t   ���	#FF74AC93$��16(�6�    �� TextAlignment$ Center=�6�   $��Medium"�6�   �� 5      $
��Wrap=�6   $��Ajouter des crédits��6,   $	��Top=�6G   $
��Auto��6_   $��26��6l   ���	#FF74AC93$��16(�6�   $ Center=�6�   $��Medium"�6�   $��	0,185,0,0q�6�   �� 5      -   5         ��Name$ txt_add_credit��5      $��AJOUTER��6-   $��77,349,86,0q�6d   $	��Top=�6y   $	��213��6�   $��27��6�   ���	#FF6C6969$��16(�6�   ���	#FF74AC93��6?   �� *AddCreditCommand+6�   �� 5       -   5        $ cmb_mode_paiement��5       $
��Left=�62   $��
77,216,0,0q�6y   $	��Top=�6�   $	��213��6�   	 �� SelectedItem	 6M   �� *ModeAddCreditActif+.@�5!      ��Carte bancaire��60   ��5"      Chèque��6(   ��5#      Espèce��6(   5$      �� 5%      -   5        
 ��Name$
 txt_num_cheque��5%      $
��Left=�6J   $��27��6e   $��
77,243,0,0q�6q    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $	��213��6�   ��6/   �� *	NumCheque+6�   �� 5&      -   5        $
 txt_montant��5&      $
��Left=�6D   $��27��6_   $��
77,275,0,0q�6k   $
 Wrap=�6   $	��Top=�6�   $	��213��6�   ��6+   �� *	Montant+6�   �� 5'      -   5        $
 txt_commentaire��5'      $
��Left=�6L   $��27��6g   $��
77,307,0,0q�6s   $
 Wrap=�6�   $	��Top=�6�   $	��213��6�   ��6/   �� *Commentaire+6�   �� 5(      -	   5         ��Name$ lbl_date��5(      $��	51,26,0,0q�6*   $
��Wrap=�6=   $
��Date��6Q   $	��Top=�6]   ���	#FF74AC93$��14(�6�    J� RenderTransformOrigin$ 0.828,-0.947&�6�   �� 5)      -
   5        $ lbl_montant��5)      $��
275,26,0,0q�6-   $
��Wrap=�6A   $��Montant��6U   $	��Top=�6d   ���	#FF74AC93$��14(�6�   �� 5*      -   5         ��Name$ lst_transactions��5*      $
��Left=�61   $
 TrueY�6�   $	��121��6�   $��	34,45,0,0q�6�   $	��Top=�6�   $	��313��6�   ��6L   �� *LesHistoriques+	 6s   �� *TransactionActive+��6  �� *CodeCouleurHistorique+.  (System.Windows.Interactivity.Interaction   Triggers 5+      /  )System.Windows.Interactivity.EventTrigger  5,         	EventName$ MouseDoubleClick��6%   .  (System.Windows.Interactivity.TriggerBase   Actions. 5-      6  0System.Windows.Interactivity.InvokeCommandAction     CommandParameter$ ViewImportProgress��6\      Command 60   �� *DetailHistoriqueCommand+6\   5.      50      51      �� 52   
   -   5        $
 txt_nom��52      $
��Left=�6E   $��26��6`   $��
24,101,0,0q�6l   $
 Wrap=�6�   $	��Top=�6�   $	��318��6�   $��1q�6�   {�6#   �� *IsEnableNom+��6�   �� *Nom+6�   �� 53   
   -   5        $
 
txt_prenom��53      $
��Left=�6K   $��25��6f   $��
24,138,0,0q�6r   $
 Wrap=�6�   $	��Top=�6�   $	��318��6�   {�6&   �� *IsEnablePrenom+��6�   �� *Prenom+6�   �� 54   
   -   5        $ 	cmb_ville��54      $
 TrueY�6�   $
��Left=�6�   $��
24,176,0,0q�6�   $	��Top=�6�   $	��318��6  $��26��6    �� 
IsEditable . {�6&   �� *IsEnableVille+��6J   �� *	Lesvilles+ �� Text 6l   �� *
Ville_id+	 6�   �� *
Ville_id+6,  �� 55   
   -   5        $
 txt_tel��55      $
��Left=�6E   $��25��6`   $��
24,214,0,0q�6l   $
 Wrap=�6�   $	��Top=�6�   $	��318��6�   {�6#   �� *IsEnableTel+��6�   �� *Tel+6�   �� 56   
   -   5        $
 txt_mail��56      $
��Left=�6G   $��27��6b   $��
24,254,0,0q�6n   $
 Wrap=�6�   $	��Top=�6�   $	��318��6�   {�6$   �� *IsEnableMail+��6�   �� *Mail+6�   �� 57   
   -   5        $ btn_archive��57      $��ARCHIVER��6&   $
��Left=�6\   $��
43,331,0,0q�6w   $	��Top=�6�   $	��156��6�   $��27��6�   ���	#FF6C6969$��16(�6�   ���	#FF74AC93��69   �� *ArchiveCommand+6�   �� 58   
   -   5        $ 
btn_modify��58      $��MODIFIER��6%   $
��Left=�6�   $��
43,295,0,0q�6�   $	��Top=�6�   $	��156��6�   $��27��6�   ���	#FF6C6969$��16(�6  ���	#FF74AC93y�68   �� *BoutonVisible+ �� 	Converter# [  6  �� 59   
   -   5        $ btn_add��59      $��AJOUTER LE CLIENT��6G   $
��Left=�6�   $��
43,295,0,0q�6�   $	��Top=�6�   $	��156��6  $��27��6  ���	#FF6C6969$��16(�6=  ���	#FF74AC93��6"   �� *AddCommandClient+y�6c   �� *AutreBoutonVisible+# [  6K  �� 5:   
   -   5         ��Name$ soldes��5:      $��SOLDES :��6    $
��Left=�63   $��
10,375,0,0q�6N   $	��Top=�6b   $��34��6z   $	��101��6�   ���	#FF74AC93$��SemiBold"�6�   $��22(�6�   �� 5;   
   -   5        $ 
lbl_soldes��5;      $
��Left=�6?   $��111,375,0,0q�6Z   $	��Top=�6o   $��34��6�   $��88��6�   
���   �$��SemiBold"�6�   $��22(�6�   ��6$   �� *Soldes+6�   �� 5<   
   $
��Left=�6   $��53��6+   $��242,305,0,0q�67   $	��Top=�6L   $��59��6d   $��
delete.png��6o    � Cursor$
 Hand��6�    � ToolTip$&  Supprime tous les champs remplis��6�    5=        5>      $ MouseLeftButtonDown��6!   . 5?        $ ViewImportProgress��6O    6,   �� *ViderLesChamps+6O   5@      5B      �� 5C   
   $
��Left=�6   $��26��6+   $��354,101,0,0q�67   $	��Top=�6L   $��34��6d   $��edit.png��6o   $
 Hand��6�   $ Modifier le champ��6�    5D        5E      $ MouseLeftButtonDown��6!   . 5F        $ ViewImportProgress��6I    6,   �� *
ViderNom+6I   5G      5I      �� 5J   
   $
��Left=�6   $��26��6+   $��354,138,0,0q�67   $	��Top=�6L   $��34��6d   $��edit.png��6o   $
 Hand��6�   $ Modifier le champ��6�    5K        5L      $ MouseLeftButtonDown��6!   . 5M        $ ViewImportProgress��6L    6,   �� *ViderPrenom+6L   5N      5P      �� 5Q   
   $
��Left=�6   $��26��6+   $��354,176,0,0q�67   $	��Top=�6L   $��34��6d   $��edit.png��6o   $
 Hand��6�   $ Modifier le champ��6�    5R        5S      $ MouseLeftButtonDown��6!   . 5T        $ ViewImportProgress��6K    6,   �� *
ViderVille+6K   5U      5W      �� 5X   
   $
��Left=�6   $��26��6+   $��354,214,0,0q�67   $	��Top=�6L   $��34��6d   $��edit.png��6o   $
 Hand��6�   $ Modifier le champ��6�    5Y        5Z      $ MouseLeftButtonDown��6!   . 5[        $ ViewImportProgress��6I    6,   �� *
ViderTel+6I   5\      5^      �� 5_   
   $
��Left=�6   $��26��6+   $��354,255,0,0q�67   $	��Top=�6L   $��34��6d   $��edit.png��6o   $
 Hand��6�   $ Modifier le champ��6�    5`        5a      $ MouseLeftButtonDown��6!   . 5b        $ ViewImportProgress��6J    6,   �� *	ViderMail+6J   5c      5e      �� 5f   
   $��DESARCHIVER��6   $
��Left=�6'   $	��Top=�6B   $	��140��6Z   $��33��6f   ���	#FF6C6969$��16(�6�   ���	#FF74AC936�   5j      5k      