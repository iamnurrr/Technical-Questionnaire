import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt
from sklearn.impute import SimpleImputer
from sklearn.utils import resample
from sklearn.metrics import classification_report, confusion_matrix, roc_auc_score
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier

# Load the data
df = pd.read_csv('dataset.csv')

# Display basic information and check for missing values
print(df.info())
print(df.isnull().sum())

plt.figure(figsize=(12, 6))
plt.suptitle('Scatter Plot of Features', fontsize=16)
for i, column in enumerate(df.columns[:-1]):
    plt.subplot(2, 2, i+1)
    sns.histplot(df[column], kde=True)
    plt.title(column)
    plt.xlabel('value')
    plt.ylabel('Density')

plt.tight_layout()
plt.show()


# Pairplot to visualize feature distribution and relationships
sns.pairplot(df, hue="isVirus")
plt.suptitle("Pairplot of Features Colored by Virus Label", y=1.02)

# Count plot of the target variable
plt.figure(figsize=(6, 4))
sns.countplot(x='isVirus', data=df)
plt.title('Count of Virus and Non-Virus Labels')
plt.show()


# Impute missing values with the mean
imputer = SimpleImputer(strategy='mean')
df_imputed = pd.DataFrame(imputer.fit_transform(df.drop(columns='isVirus')))
df_imputed.columns = df.columns[:-1]
df_imputed['isVirus'] = df['isVirus']

print(df_imputed.isnull().sum())

# Plot histograms of features after imputation
plt.figure(figsize=(12, 6))
plt.suptitle('After Imputation', fontsize=16)
for i, column in enumerate(df_imputed.columns[:-1]):
    plt.subplot(2, 2, i+1)
    sns.histplot(df_imputed[column], kde=True)
    plt.title(column)
    plt.xlabel('Value')
    plt.ylabel('Density')

plt.tight_layout()
plt.show()


# Separate majority and minority classes
majority = df_imputed[df_imputed.isVirus == False]
minority = df_imputed[df_imputed.isVirus == True]

# Upsample minority class
minority_upsampled = resample(minority, replace=True, n_samples=len(majority), random_state=42)

# Combine majority and upsampled minority
balanced_data = pd.concat([majority, minority_upsampled])

# Display new class counts
print(balanced_data['isVirus'].value_counts())


X = balanced_data.drop(columns='isVirus')
y = balanced_data['isVirus']

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)


model = RandomForestClassifier(random_state=42)
model.fit(X_train, y_train)


y_pred = model.predict(X_test)

print(confusion_matrix(y_test, y_pred))
print(classification_report(y_test, y_pred))
print('ROC AUC Score:', roc_auc_score(y_test, y_pred))

# Plot confusion matrix after training the model

sns.heatmap(confusion_matrix(y_test, y_pred), annot=True, fmt='d', cmap='Blues')
plt.title('Confusion Matrix')
plt.xlabel('Predicted')
plt.ylabel('True')
plt.show()
