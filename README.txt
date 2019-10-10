GameAI
Homework (Project) 3: 
Haoran Hu
Eric Xu


0: Initial state: Swapn 20 trees by reading positions from a given text file.
1: The hunter appears then wanders in the map.
2: The wolf appears and wanders in the map.
3: The hunter and the wolf wander until the hunter spots the wolf and believes it is his target. 
    The Wolf evades and the hunter pursues.
4: Red appears and walks to Granny's house using path following algorithm with obstacle avoidence.
5: Wolf appears and pursues Red.
6: This stage can only be triggered automatically when wolf and red collide in stage 5.
7: Hunter appears again and runs to Granny’s house.
8: 

a.hunter and wolf collide on the way(not at house), wolf gets killed. Wolf must disppear.
b.wolf arrive at house first: Granny gets killed.
c.red arrive at house first: shut the door. Hunter and wolf will collide at the house, wolf must disapper.
d.hunter arrive at house first: wait for the wolf. When they collide, wolf must disappear. 