GameAI
Homework Project3(Red,Hunter and Wolf) : 
Haoran Hu
Eric Xu


Description: 
    This project consits two little stories. The stages(0,1,2,3,4,5,7) of story can be triggered by user input from keyboard.
    Note that stage 6 will be triggered auotomatically when certain condition meets in stage5.
    
   
    <<< First story (0,1,2,3)>>>
    0: Initial state: Swapn 20 trees by reading positions from a given text file.
    1: The hunter appears then wanders in the map.
    2: The wolf appears and wanders in the map.
    3: The hunter and the wolf wander until the hunter spots the wolf(within certain distance) and believes it is his target. 
        The Wolf evades and the hunter pursues.
        Upon collision, they disappear after 2 seconds.

    <<<  Second story (4,5,6,7) >>>
    4: Red appears and walks to Granny's house using path following algorithm with obstacle avoidence.
    5: Wolf appears and pursues Red.
        Once they meet, they both stop and talk for a few seconds, then stage6
        6: (This stage can only be triggered automatically when wolf and red collide in stage 5)
            Wolf runs to Granny’s house (dynamic pursue with arrive using slow radius),
            while Red continues on her path to Grandma’s.
    7: Hunter appears again and runs to Granny’s house.

    As stated above, all three agent(Red, hunter and wolf) will eventually arrive at the house.
    Depending on the sequence of arriving, the story have corresbonding endings:

    Wolf    Hunter  Red                 Endings
    1       2       3       Wolf eats granny => Hunter kills wolf => Red finds granny's blood on the floor
    1       3       2       Wolf eats granny => Red finds granny's blood on the floor => Hunter kills wolf
    2       1       3       Hunter guards the house => Wolf gets killed => (Red arrives)
    2       3       1       Red escape with granny from backyard => Wolf finds no one in the house => Hunter kills wolf
    3       1       2       Hunter guards the house => Red escape with granny from backyard => Wolf gets killed by hunter
    3       2       1       Red escape with granny from backyard => Hunter guards the house =>  Wolf gets killed by hunter


