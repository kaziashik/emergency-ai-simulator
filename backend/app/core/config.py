"""Application settings loaded from environment variables."""

from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    """Runtime configuration for the EmergencyAI backend."""

    app_name: str = "EmergencyAI"
    host: str = "127.0.0.1"
    port: int = 8000
    llm_enabled: bool = False
    openai_api_key: str = ""

    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        extra="ignore",
    )


settings = Settings()
