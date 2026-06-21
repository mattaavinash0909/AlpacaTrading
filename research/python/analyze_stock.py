import sys
import json
import os
from datetime import datetime, timedelta, timezone

import pandas as pd

from alpaca.data.historical import StockHistoricalDataClient
from alpaca.data.requests import StockBarsRequest
from alpaca.data.timeframe import TimeFrame
from alpaca.data.enums import DataFeed, Adjustment


def to_float(value):
    if pd.isna(value):
        return None
    return float(value)


def analyze_stock(symbol: str):
    symbol = symbol.upper().strip()

    api_key = os.getenv("APCA_API_KEY_ID")
    api_secret = os.getenv("APCA_API_SECRET_KEY")

    if not api_key or not api_secret:
        return {
            "symbol": symbol,
            "status": "Error",
            "message": "Alpaca API key or secret is missing. Set APCA_API_KEY_ID and APCA_API_SECRET_KEY.",
            "generatedAt": datetime.now(timezone.utc).isoformat()
        }

    try:
        client = StockHistoricalDataClient(api_key, api_secret)

        end_date = datetime.now(timezone.utc) - timedelta(minutes=20)
        start_date = end_date - timedelta(days=220)

        request = StockBarsRequest(
            symbol_or_symbols=symbol,
            timeframe=TimeFrame.Day,
            start=start_date,
            end=end_date,
            feed=DataFeed.IEX,
            adjustment=Adjustment.ALL
        )

        bars = client.get_stock_bars(request)

        df = bars.df

        if df.empty:
            return {
                "symbol": symbol,
                "status": "Error",
                "message": "No Alpaca market data found for symbol.",
                "generatedAt": datetime.now(timezone.utc).isoformat()
            }

        # Alpaca returns MultiIndex dataframe: symbol + timestamp.
        # For one symbol, select only that symbol's rows.
        if isinstance(df.index, pd.MultiIndex):
            df = df.loc[symbol]

        df = df.sort_index()

        if len(df) < 50:
            return {
                "symbol": symbol,
                "status": "Error",
                "message": "Not enough historical bars to calculate SMA50.",
                "generatedAt": datetime.now(timezone.utc).isoformat()
            }

        df["SMA20"] = df["close"].rolling(window=20).mean()
        df["SMA50"] = df["close"].rolling(window=50).mean()

        latest = df.iloc[-1]

        close_price = to_float(latest["close"])
        sma20 = to_float(latest["SMA20"])
        sma50 = to_float(latest["SMA50"])
        volume = int(latest["volume"]) if not pd.isna(latest["volume"]) else 0

        signal = "HOLD"
        reason = "Not enough trend confirmation."

        if close_price is not None and sma20 is not None and sma50 is not None:
            if close_price > sma20 and sma20 > sma50:
                signal = "BUY"
                reason = "Price is above SMA20 and SMA20 is above SMA50. Trend is positive."
            elif close_price < sma20 and sma20 < sma50:
                signal = "SELL"
                reason = "Price is below SMA20 and SMA20 is below SMA50. Trend is weak."
            else:
                signal = "HOLD"
                reason = "Mixed trend. No clear buy or sell signal."

        return {
            "symbol": symbol,
            "status": "Success",
            "dataSource": "Alpaca Market Data - IEX",
            "latestClose": round(close_price, 2) if close_price is not None else None,
            "sma20": round(sma20, 2) if sma20 is not None else None,
            "sma50": round(sma50, 2) if sma50 is not None else None,
            "volume": volume,
            "signal": signal,
            "reason": reason,
            "generatedAt": datetime.now(timezone.utc).isoformat()
        }

    except Exception as ex:
        return {
            "symbol": symbol,
            "status": "Error",
            "message": str(ex),
            "generatedAt": datetime.now(timezone.utc).isoformat()
        }


if __name__ == "__main__":
    stock_symbol = sys.argv[1] if len(sys.argv) > 1 else "AAPL"
    result = analyze_stock(stock_symbol)
    print(json.dumps(result))