using UnityEngine;
/// <summary>
/// 애니메이션 파라미터값이 존재하는지 안하는지 확인해주는 코드
/// </summary>
public static class AnimatorExtensions
{
    public static bool HasParameter(this Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }
}
