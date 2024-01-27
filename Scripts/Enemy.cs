using globalgamejam2024.Shared;
using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	//only take damage from a punch once
	protected int RecievedPunchID = 0;
    protected float Health = 1;
    [Export] float MaxHealth = 1f;
    [Export] public int Speed { get; set; } = 100;
    protected int MeleeRange = 50;
    protected int MeleeRangeSqrd = 50 * 50;

    [Export] double IdleTime = 0.5f;
    double tLeftIdle = 0f;

    [Export] AnimatedSprite2D IdleAnim;
    [Export] AnimatedSprite2D WalkAnim;
    [Export] AnimatedSprite2D PunchAnim;

    [Export] float BaseDamage = 1f;
    [Export] Area2D PunchArea2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //some basic settings
        Health = MaxHealth;

        //initial animation settings
        WalkAnim.Play();
        IdleAnim.Play();
        PunchAnim.Visible = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (tLeftIdle > 0)
        {
            tLeftIdle -= delta;
            if (tLeftIdle <= 0)
            {
                WalkAnim.Visible = true;
                IdleAnim.Visible = false;
                tLeftIdle = 0;
            }
        }
    }

    public void MoveTo(Vector2 newPos)
	{
        //GD.Print("Enemy::MoveTo(" + newPos.X + "," + newPos.Y + ")");
        Position = newPos;
	}

	public override void _PhysicsProcess(double delta)
    {
        //are we idling?
        if(tLeftIdle > 0)
        {
            return;
        }

        //where is the player?
        Vector2 moveVector = new Vector2();
        if ((GGJ.player.Position - Position).LengthSquared() > MeleeRangeSqrd)
        {
            moveVector = (GGJ.player.Position - Position).Normalized() * Speed * (float)delta;
            //WalkAnim.Visible = true;
            //IdleAnim.Visible = false;
        }
        else
        {
            //WalkAnim.Visible = false;
            //IdleAnim.Visible = true;
        }
        KinematicCollision2D collisionInfo = MoveAndCollide(moveVector);

        //if we bumped into something, go idle for a short while
        if(collisionInfo != null && GGJ.random.Next(2) == 1) 
        {
            GoIdle();
            WalkAnim.Visible = false;
            IdleAnim.Visible = true;
        }
    }

    public void GoIdle()
    {
        tLeftIdle = IdleTime;
    }


    public void ReceivePunch(float damage)
	{
        if (GGJ.CurrentPunchID != RecievedPunchID && Health > 0)
        {
			//only process a hit once per punch
            RecievedPunchID = GGJ.CurrentPunchID;
            //GD.Print("Ow! " + RecievedPunchID);
            Health -= damage;

            if(Health <= 0)
            {
                //die
                GGJ.mobController.KillEnemy(this);
            }
        }
    }

    public void _on_screen_exited()
    {
        //die
        //GD.Print("Goodbye");
        GGJ.mobController.KillEnemy(this);
    }

    public bool isPunching()
    {
        return PunchAnim.IsPlaying();
    }

    public bool tryPunch()
    {
        //are we idling?
        if (tLeftIdle > 0)
        {
            return false;
        }
        //loop over all the overlapping enemies and apply a punch
        Godot.Collections.Array<Node2D> overlappingNodes = PunchArea2D.GetOverlappingBodies();
        foreach (Node2D checkNode in overlappingNodes)
        {
            //PhysicsBody2D physicsBody = (PhysicsBody2D)checkNode;
            //GD.Print(checkNode.Name + " " + physicsBody.CollisionLayer + "/" + physicsBody.CollisionMask);

            //this is safe so long as the collision masks are properly setup
            if(checkNode.Name == "Player")
            {
                Player punchedPlayer = (Player)checkNode;
                punchedPlayer.ReceivePunch(BaseDamage);
            }
        }

        //play the correct animation
        PunchAnim.Play();
        PunchAnim.Visible = true;
        WalkAnim.Visible = false;

        return true;
    }

    public void _on_punch_anim_finish()
    {
        PunchAnim.Visible = false;
        WalkAnim.Visible = true;
    }
}
