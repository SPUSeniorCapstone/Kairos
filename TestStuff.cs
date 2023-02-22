

Entity(){
    if(avoidance){
        Avoid();
    }

    if (idle){
        Idle()
    }
    else{
       PerformTask()
    }
}


 Idle(){

    if(enemy in range){
        Attack()
    }
 }

 PerformTask(){
    if(useCG){
        MoveWithCommandGroup();  
    }
    else if(useTarget){
        MoveTowardsTarget();
    }
 }

MoveWithCommandGroup(){
    Move...
}