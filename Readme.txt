Democleaner2

 A program for processing (moving / deleting) demo files created when recording card passes in Quake3.

 Supports many options:
 - Ability to process also subdirectories
 At the very beginning, depending on the type of the expected directory processing (with subdirectories or not), the program passes through all the files and then, in a certain way, processes the found files.

 - cleaning (deleting / moving files) can occur both for a better time on the map, and for the best time of each player
 - There is a choice of the number of files with the best time that you want to keep.
 (for example, you can save the two best demos of each player)
 - It is possible to process mdf timings as df, that is, if there are two types of records, only the best time will be selected.
 - several possibilities of processing records with slow time:
 Or delete, or move (all in 1 folder), or skip, for example, in order to process only badly-named demos. 
 
 Department "Demo mover"
 Allows you to move files in a specific folder by subdirectories.
 Actual in large file directories.
 Its main function is not only to move files in alphabetical order, but also to separate records in subdirectories, and also to subdivide the subdirectories themselves into subdirectories!
 Mainly because this modification of the game can not display more than a certain number of records (1024) and more than 64 directories in the selection list.
 Because of this, you have to divide them into subdirectories for viewing, and if it is done manually, there is a lot of confusion, and it also takes time.
 Basically, that's why this program was developed for the algorithm of file distribution algorithm.
 - Also allows you to move only the records of a certain player

 Additional options apply both to the part with the removal of files, and to the part with the move.
 - handle all the incorrectly named entries (grab, move, skip)
 - Delete empty directories.  After cleaning or moving records, empty directories can be created, which the program must delete.
 - Delete identically named files.  That is, if there are 2 identical records in 2 directories (only the name is checked, but not the content), the program leaves only the first one.  Otherwise - just skip it.