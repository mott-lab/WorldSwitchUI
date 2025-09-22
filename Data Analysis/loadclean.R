# Load ggplot 
library(ggplot2)
library(dplyr)

# Load CSV
data_basepath = "SET TO THE FULL PATH OF THE WorldSwitchUI GIT REPO"

setwd(data_basepath)

df_subjective = read.csv(file.path(data_basepath, "subjective.csv"))
df_ranking = read.csv(file.path(data_basepath, "Interface_Ranking_Counts.csv"), row.names = 1)

interaction_file_list = list.files(pattern = "\\_trial_data.csv$", recursive = TRUE)
print(interaction_file_list)

combined_data = data.frame()


for (i in 1:length(interaction_file_list)) {
  
  print(interaction_file_list[i])
  
  temp_data = read.csv(file.path(getwd(), interaction_file_list[i]))

  # interaction_df = temp_data[-nrow(temp_data), ]
  interaction_df = temp_data
    
  combined_data = rbind(combined_data, interaction_df)
}

combined_data$SearchTime = as.numeric(combined_data$SearchTime)
combined_data$RetrieveTime = as.numeric(combined_data$RetrieveTime)
combined_data$DepositTime = as.numeric(combined_data$DepositTime)
combined_data$TotalTime = as.numeric(combined_data$TotalTime)
combined_data$ErrorCount = as.numeric(combined_data$ErrorCount)

combined_data$TaskBlock = as.numeric(combined_data$TaskBlock)
combined_data$Trial = as.numeric(combined_data$Trial)

combined_data$ParticipantID = as.factor(combined_data$Participant)
combined_data$Interface = as.factor(combined_data$Technique)
combined_data$InteractionTechnique = as.factor(combined_data$InteractionMetaphor)
combined_data$PreviewPattern = as.factor(combined_data$TechniquePreview)
combined_data$TechniqueInteractionSpace = as.factor(combined_data$TechniqueInteractionSpace)
combined_data$TechniquePreivewSpace = as.factor(combined_data$TechniquePreviewSpace)

df = combined_data

sd_cutoff = 4

df_flagged = df %>%
  group_by(Interface) %>%
  mutate(
    
    mean_duration_search = mean(SearchTime, na.rm = TRUE),
    sd_duration_search = sd(SearchTime, na.rm = TRUE),
    is_outlier_search = SearchTime < (mean_duration_search - sd_cutoff * sd_duration_search) |
      SearchTime > (mean_duration_search + sd_cutoff * sd_duration_search),

    mean_duration_retrieve = mean(RetrieveTime, na.rm = TRUE),
    sd_duration_retrieve = sd(RetrieveTime, na.rm = TRUE),
    is_outlier_retrieve = RetrieveTime < (mean_duration_retrieve - sd_cutoff * sd_duration_retrieve) |
      RetrieveTime > (mean_duration_retrieve + sd_cutoff * sd_duration_retrieve),

    mean_duration_deposit = mean(DepositTime, na.rm = TRUE),
    sd_duration_deposit = sd(DepositTime, na.rm = TRUE),
    is_outlier_deposit = DepositTime < (mean_duration_deposit - sd_cutoff * sd_duration_deposit) |
      DepositTime > (mean_duration_deposit + sd_cutoff * sd_duration_deposit)
  ) %>%
  ungroup()

# print rows in original data frame
print(paste("Original data frame has", nrow(df), "rows"))

df_clean_search = df_flagged %>%
  filter(!is_outlier_search)

print("Search Time Outlier Removal:")
print(paste("Removed", nrow(df_flagged) - nrow(df_clean_search), "outlier rows"))
print(paste("Total rows in cleaned data frame:", nrow(df_clean_search)))

df_clean_retrieve = df_flagged %>%
  filter(!is_outlier_retrieve)

print("Retrieve Time Outlier Removal:")
print(paste("Removed", nrow(df_flagged) - nrow(df_clean_retrieve), "outlier rows"))
print(paste("Total rows in cleaned data frame:", nrow(df_clean_retrieve)))

df_clean_deposit = df_flagged %>%
  filter(!is_outlier_deposit)

print("Deposit Time Outlier Removal:")
print(paste("Removed", nrow(df_flagged) - nrow(df_clean_deposit), "outlier rows"))
print(paste("Total rows in cleaned data frame:", nrow(df_clean_deposit)))

df_flagged %>%
  count(is_outlier_search | is_outlier_retrieve | is_outlier_deposit)

# Visualize outliers

ggplot(df_flagged, aes(x = Interface, y = SearchTime, color = is_outlier_search)) +
  geom_jitter(width = 0.2, height = 0) +
  theme_minimal() +
  labs(title = "Task Duration per Interface", color = "Outlier")

ggplot(df_flagged, aes(x = Interface, y = RetrieveTime, color = is_outlier_retrieve)) +
  geom_jitter(width = 0.2, height = 0) +
  theme_minimal() +
  labs(title = "Task Duration per Interface", color = "Outlier")

ggplot(df_flagged, aes(x = Interface, y = DepositTime, color = is_outlier_deposit)) +
  geom_jitter(width = 0.2, height = 0) +
  theme_minimal() +
  labs(title = "Task Duration per Interface", color = "Outlier")