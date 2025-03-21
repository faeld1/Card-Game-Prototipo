using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    public void EndAttackAnimation()
    {
        player.OnAttackAnimationEnd();
    }

    public void EndSpecialAttackAnimation()
    {
        player.OnSpecialAttackAnimationEnd();
    }

    public void EndHitAnimation()
    {
        player.OnHitAnimationEnd();
    }

    public void EndSupportAnimation()
    {
        player.OnSupportAnimationEnd();
    }
}
