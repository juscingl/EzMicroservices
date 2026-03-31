{{- define "authcenter.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "authcenter.fullname" -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "authcenter.labels" -}}
app.kubernetes.io/name: {{ include "authcenter.name" . }}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version | replace "+" "_" }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/managed-by: Helm
{{- end -}}

{{- define "authcenter.selectorLabels" -}}
app.kubernetes.io/name: {{ include "authcenter.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}
