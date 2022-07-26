using NBitcoin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WalletWasabi.Blockchain.TransactionOutputs;
using WalletWasabi.Extensions;
using WalletWasabi.WabiSabi.Client.RoundStateAwaiters;
using WalletWasabi.Wallets;
using WalletWasabi.WebClients.Wasabi;

namespace WalletWasabi.WabiSabi.Client;

public class CoinJoinTrackerFactory
{
	public CoinJoinTrackerFactory(
		IWasabiHttpClientFactory httpClientFactory,
		RoundStateUpdater roundStatusUpdater,
		CancellationToken cancellationToken)
	{
		HttpClientFactory = httpClientFactory;
		RoundStatusUpdater = roundStatusUpdater;
		CancellationToken = cancellationToken;
	}

	private IWasabiHttpClientFactory HttpClientFactory { get; }
	private RoundStateUpdater RoundStatusUpdater { get; }
	private CancellationToken CancellationToken { get; }

	public CoinJoinTracker CreateAndStart(Wallet wallet, IEnumerable<SmartCoin> coinCandidates, bool restartAutomatically, bool overridePlebStop)
	{
		Money? liquidityClue = null;
		if (CoinJoinClient.GetLiquidityClue() is not null)
		{
			var lastCoinjoin = wallet.TransactionProcessor.TransactionStore.GetTransactions().OrderByBlockchain().LastOrDefault(x => x.IsOwnCoinjoin());
			if (lastCoinjoin is not null)
			{
				liquidityClue = CoinJoinClient.CalculateLiquidityClue(lastCoinjoin.Transaction, lastCoinjoin.WalletOutputs.Select(x => x.TxOut));
			}
		}

		var coinJoinClient = new CoinJoinClient(
			HttpClientFactory,
			new KeyChain(wallet.KeyManager, wallet.Kitchen),
			new InternalDestinationProvider(wallet.KeyManager),
			RoundStatusUpdater,
			wallet.KeyManager.AnonScoreTarget,
			consolidationMode: false,
			redCoinIsolation: wallet.KeyManager.RedCoinIsolation,
			feeRateMedianTimeFrame: TimeSpan.FromHours(wallet.KeyManager.FeeRateMedianTimeFrameHours),
			doNotRegisterInLastMinuteTimeLimit: TimeSpan.FromMinutes(1),
			liquidityClue: liquidityClue);

		return new CoinJoinTracker(wallet, coinJoinClient, coinCandidates, restartAutomatically, overridePlebStop, CancellationToken);
	}
}
