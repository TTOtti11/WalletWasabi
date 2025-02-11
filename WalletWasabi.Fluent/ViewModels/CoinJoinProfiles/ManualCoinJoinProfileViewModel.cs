using WalletWasabi.Fluent.Models.Wallets;

namespace WalletWasabi.Fluent.ViewModels.CoinJoinProfiles;

public class ManualCoinJoinProfileViewModel : CoinJoinProfileViewModelBase
{
	public ManualCoinJoinProfileViewModel(int anonScoreTarget, int feeRateMedianTimeFrameHours, bool redCoinIsolation)
	{
		AnonScoreTarget = anonScoreTarget;
		FeeRateMedianTimeFrameHours = feeRateMedianTimeFrameHours;
		RedCoinIsolation = redCoinIsolation;
	}

	public ManualCoinJoinProfileViewModel(IWalletSettingsModel walletSettings) : this(walletSettings.AnonScoreTarget, walletSettings.FeeRateMedianTimeFrameHours, walletSettings.RedCoinIsolation)
	{
	}

	public override string Title => "Custom";

	public override string Description => "";

	public override int AnonScoreTarget { get; }

	public override int FeeRateMedianTimeFrameHours { get; }
	public override bool RedCoinIsolation { get; }
}
